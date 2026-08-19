using IPFees.Core.FeeCalculation;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace IPFees.Web.Services
{
    public class DatabaseResetService : BackgroundService
    {
        private readonly IMongoDatabase database;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<DatabaseResetService> logger;
        private readonly string dataFolder;
        // Jurisdictions, regional bases and fee schedules come from the IPFLang package,
        // so they move forward with the language rather than with a file checked in here.
        // Service fee levels are a local commercial setting and stay file-driven.
        private readonly (int Index, string FileName, string CollectionName)[] fileMappings =
        [
            (0, "servicefees.json", "ServiceFees")
        ];

        public DatabaseResetService(IMongoClient mongoClient, IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<DatabaseResetService> logger)
        {
            this.scopeFactory = scopeFactory;
            var mongoUrl = new MongoUrl(configuration.GetValue<string>("ConnectionStrings:MongoDbConnection"));
            database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            this.logger = logger;
            var configDataFolder = configuration.GetValue<string>("DataFolder");
            dataFolder = Path.Combine(Directory.GetCurrentDirectory(), configDataFolder ?? "wwwroot/data");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run immediately on startup, then every 6 hours
            await PerformDatabaseReset(stoppingToken);

            var timer = new PeriodicTimer(TimeSpan.FromHours(6));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await PerformDatabaseReset(stoppingToken);
            }
        }

        private async Task PerformDatabaseReset(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Starting database reset at {Time}", DateTime.Now);

                foreach (var item in fileMappings)
                {
                    string collectionName = item.CollectionName;
                    string filePath = Path.Combine(dataFolder, item.FileName);

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
                    var collection = database.GetCollection<BsonDocument>(collectionName);
                    await collection.Database.DropCollectionAsync(collectionName, stoppingToken);
                    await collection.InsertManyAsync(documents, null, stoppingToken);

                    logger.LogInformation("Inserted {Count} documents into {Collection}.", documents.Count, collectionName);
                }

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
        /// Load the jurisdiction corpus shipped inside the IPFLang package. Existing entries are
        /// refreshed in place rather than dropped, so a service fee level an operator changed
        /// survives the reset.
        /// </summary>
        private async Task SeedCorpus(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<ICorpusSeeder>();

            var report = await seeder.SeedAsync(stoppingToken);

            logger.LogInformation(
                "Seeded {Jurisdictions} jurisdictions and {Bases} regional bases from the IPFLang corpus; removed {Superseded} superseded fee documents.",
                report.Jurisdictions, report.Bases, report.Superseded);

            foreach (var error in report.Errors)
            {
                logger.LogError("Corpus seeding: {Error}", error);
            }
        }
    }
}
