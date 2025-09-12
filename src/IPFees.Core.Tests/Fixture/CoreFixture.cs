using DotNet.Testcontainers.Containers;
using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IPFees.Core.Tests.Fixture
{
    public class CoreFixture : IAsyncDisposable
    {
        public ExchangeRateFetcher CurrencyConverter { get; set; }
        public FeeRepository FeeRepository { get; set; }
        public JurisdictionRepository JurisdictionRepository { get; set; }
        public ModuleRepository ModuleRepository { get; set; }
        public SettingsRepository SettingsRepository { get; set; }        
        public DataContext DbContext { get; private set; }
        private readonly IContainer mongoContainer;        
        private readonly string connectionString = string.Empty;
        private readonly string ExchangeApiKey = "1234567890"; // Get an actual API key from https://www.exchangerate-api.com/

        public CoreFixture()
        {
            try
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));                
            }
            catch (Exception ex) when (ex.Message.Contains("already been registered"))
            {
                // Ignore: Already registered
                Console.WriteLine("Serializer already registered—skipping.");
            }

            // Start the test MongoDb instance            
            mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .WithPortBinding(27017, true)
                .WithEnvironment("MONGO_INITDB_DATABASE", "IPFeesTest")
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "root")
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "password")
                .Build();

            // Start the container
            mongoContainer.StartAsync().GetAwaiter().GetResult();

            // Use the dynamically assigned host port
            connectionString = $"mongodb://root:password@localhost:{mongoContainer.GetMappedPublicPort(27017)}/IPFeesTest?authSource=admin&authMechanism=SCRAM-SHA-1";

            // Build database context based on the connection string
            DbContext = new DataContext(connectionString);

            CurrencyConverter = new ExchangeRateFetcher(ExchangeApiKey);
            FeeRepository = new FeeRepository(DbContext);
            JurisdictionRepository = new JurisdictionRepository(DbContext);
            ModuleRepository = new ModuleRepository(DbContext);
            SettingsRepository = new SettingsRepository(DbContext);
        }

        public async ValueTask DisposeAsync()
        {
            // Stop and clean up the container
            await mongoContainer.DisposeAsync();
        }
    }
}
