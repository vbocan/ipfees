using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace IPFees.Web.Services
{
    public class DatabaseResetService : BackgroundService
    {
        private readonly IMongoDatabase database;
        private readonly ILogger<DatabaseResetService> logger;
        private readonly string dataFolder;
        private readonly (int Index, string FileName, string CollectionName)[] fileMappings =
        [
            (0, "servicefees.json", "ServiceFees"),
            (1, "jurisdictions.json", "Jurisdictions"),
            (2, "modules.json", "Modules"),
            (3, "fees.json", "Fees")
        ];

        public DatabaseResetService(IMongoClient mongoClient, IConfiguration configuration, ILogger<DatabaseResetService> logger)
        {
            var mongoUrl = new MongoUrl(configuration.GetValue<string>("ConnectionStrings:MongoDbConnection"));
            database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            this.logger = logger;
            var configDataFolder = configuration.GetValue<string>("DatabaseReset:DataFolder");
            dataFolder = Path.Combine(Directory.GetCurrentDirectory(), configDataFolder ?? "wwwroot/data");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run every 6 hour
            var timer = new PeriodicTimer(TimeSpan.FromHours(6));

            while (await timer.WaitForNextTickAsync(stoppingToken))
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
        }
    }
}
