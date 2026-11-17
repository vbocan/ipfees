using IPFees.Core.FeeCalculation;
using IPFees.Core.Repository;
using IPFees.Web.Data;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace IPFees.Web.Services
{
    /// <summary>
    /// Loads the reference data into the store. With <see cref="DatabaseResetSettings.Enabled"/>
    /// (the public sandbox), the store is returned to the reference corpus at start-up and every
    /// <see cref="DatabaseResetSettings.IntervalHours"/> hours. Without it (a firm's deployment),
    /// only an empty store is seeded, and what the firm changes afterwards stays.
    /// </summary>
    public class DatabaseResetService : BackgroundService
    {
        private readonly IMongoDatabase database;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<DatabaseResetService> logger;
        private readonly DatabaseResetSettings settings;
        private readonly string dataFolder;
        // Jurisdictions, regional bases and fee schedules come from the IPFLang package,
        // so they move forward with the language rather than with a file checked in here.
        // Service fee levels are a local commercial setting and stay file-driven.
        private readonly (int Index, string FileName, string CollectionName)[] fileMappings =
        [
            (0, "servicefees.json", "ServiceFees")
        ];

        public DatabaseResetService(IMongoClient mongoClient, IConfiguration configuration, IOptions<DatabaseResetSettings> settings,
            IServiceScopeFactory scopeFactory, ILogger<DatabaseResetService> logger)
        {
            this.scopeFactory = scopeFactory;
            var mongoUrl = new MongoUrl(configuration.GetValue<string>("ConnectionStrings:MongoDbConnection"));
            database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            this.logger = logger;
            this.settings = settings.Value;
            dataFolder = Path.Combine(Directory.GetCurrentDirectory(),
                string.IsNullOrWhiteSpace(this.settings.DataFolder) ? "wwwroot/data" : this.settings.DataFolder);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!settings.Enabled)
            {
                // A firm's deployment: seed a store that is still empty, then leave it alone.
                await SeedEmptyStore(stoppingToken);
                return;
            }

            // The public sandbox: reset on start-up, then at every interval.
            await PerformDatabaseReset(stoppingToken);
            if (settings.IntervalHours <= 0)
            {
                logger.LogInformation("Database reset runs at start-up only (DatabaseReset:IntervalHours is {Hours}).", settings.IntervalHours);
                return;
            }

            var timer = new PeriodicTimer(TimeSpan.FromHours(settings.IntervalHours));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await PerformDatabaseReset(stoppingToken);
            }
        }

        /// <summary>
        /// With the reset off, load the service fee levels and the corpus only where the store has
        /// none, so a new deployment starts from the reference corpus and an existing one keeps
        /// what its users changed.
        /// </summary>
        private async Task SeedEmptyStore(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Database reset is off (DatabaseReset:Enabled is false); seeding only what the store lacks.");
                await LoadDataFiles(onlyIntoEmptyCollections: true, stoppingToken);

                using var scope = scopeFactory.CreateScope();
                var jurisdictions = scope.ServiceProvider.GetRequiredService<IJurisdictionRepository>();
                if ((await jurisdictions.GetJurisdictions()).Any())
                {
                    logger.LogInformation("The store already holds jurisdictions; the corpus is not reloaded.");
                    return;
                }

                var report = await scope.ServiceProvider.GetRequiredService<ICorpusSeeder>().SeedAsync(stoppingToken);
                logger.LogInformation(
                    "Seeded {Jurisdictions} jurisdictions and {Bases} regional bases from the IPFLang corpus into an empty store.",
                    report.Jurisdictions, report.Bases);
                foreach (var error in report.Errors)
                {
                    logger.LogError("Corpus seeding: {Error}", error);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Seeding the empty store failed: {Error}", ex.Message);
            }
        }

        private async Task PerformDatabaseReset(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Starting database reset at {Time}", DateTime.Now);

                await LoadDataFiles(onlyIntoEmptyCollections: false, stoppingToken);

                await SeedCorpus(stoppingToken);

                // Verify collection counts
                foreach (var item in fileMappings)
                {
                    var collection = database.GetCollection<BsonDocument>(item.CollectionName);
                    var count = await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, null, stoppingToken);
                    logger.LogInformation("{Collection} has {Count} documents after reset.", item.CollectionName, count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Database reset failed: {Error}", ex.Message);
            }
        }

        /// <summary>
        /// Replace each collection mapped to a data file with the file's documents, or, with
        /// <paramref name="onlyIntoEmptyCollections"/>, fill only the collections that are empty.
        /// </summary>
        private async Task LoadDataFiles(bool onlyIntoEmptyCollections, CancellationToken stoppingToken)
        {
            foreach (var item in fileMappings)
            {
                string collectionName = item.CollectionName;
                string filePath = Path.Combine(dataFolder, item.FileName);
                var collection = database.GetCollection<BsonDocument>(collectionName);

                if (onlyIntoEmptyCollections &&
                    await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, null, stoppingToken) > 0)
                {
                    continue;
                }

                if (!File.Exists(filePath))
                {
                    logger.LogWarning("File {FileName} not found.", item.FileName);
                    continue;
                }

                // Read JSON file
                string jsonContent = await File.ReadAllTextAsync(filePath, stoppingToken);

                // Validate JSON array using System.Text.Json
                JsonDocument jsonDoc;
                try
                {
                    jsonDoc = JsonDocument.Parse(jsonContent);
                    if (!jsonDoc.RootElement.ValueKind.Equals(JsonValueKind.Array))
                    {
                        logger.LogError("File {FileName} is not a JSON array.", item.FileName);
                        continue;
                    }
                }
                catch (JsonException ex)
                {
                    logger.LogError("Invalid JSON in {FileName}: {Error}", item.FileName, ex.Message);
                    continue;
                }

                // Parse each JSON object into BsonDocument
                var documents = new List<BsonDocument>();
                foreach (var element in jsonDoc.RootElement.EnumerateArray())
                {
                    try
                    {
                        string jsonString = element.GetRawText();
                        var bsonDoc = BsonDocument.Parse(jsonString); // Handles $oid, $binary, $date
                        documents.Add(bsonDoc);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("Failed to parse document in {FileName}: {Error}", item.FileName, ex.Message);
                    }
                }

                if (documents.Count == 0)
                {
                    logger.LogWarning("No valid documents found in {FileName}.", item.FileName);
                    continue;
                }

                // Drop and repopulate collection
                await collection.Database.DropCollectionAsync(collectionName, stoppingToken);
                await collection.InsertManyAsync(documents, null, stoppingToken);

                logger.LogInformation("Inserted {Count} documents into {Collection}.", documents.Count, collectionName);
            }
        }

        /// <summary>
        /// Return the store to the jurisdiction corpus shipped inside the IPFLang package. The
        /// public instance is a sandbox anyone may edit, so a reset removes every fee definition,
        /// module and jurisdiction a visitor added, puts back what a visitor changed, and returns
        /// every jurisdiction to the default service fee level.
        /// </summary>
        private async Task SeedCorpus(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<ICorpusSeeder>();

            var reset = await seeder.ResetAsync(stoppingToken);
            var report = reset.Seed;

            logger.LogInformation(
                "Removed {Definitions} fee definitions, {Modules} modules and {Jurisdictions} jurisdictions the IPFLang corpus does not define.",
                reset.Definitions, reset.Modules, reset.Jurisdictions);
            logger.LogInformation(
                "Seeded {Jurisdictions} jurisdictions and {Bases} regional bases from the IPFLang corpus; removed {Superseded} superseded fee documents.",
                report.Jurisdictions, report.Bases, report.Superseded);

            foreach (var error in reset.Errors.Concat(report.Errors))
            {
                logger.LogError("Corpus seeding: {Error}", error);
            }

            foreach (var schedule in report.RefusedSchedules)
            {
                logger.LogWarning("Corpus seeding: PCT-{Code} is refused by the save gate and not served: {Findings}",
                    schedule.Code, string.Join(" | ", schedule.Findings));
            }
        }
    }
}
