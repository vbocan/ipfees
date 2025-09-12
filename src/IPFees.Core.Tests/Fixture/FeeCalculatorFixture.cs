using DotNet.Testcontainers.Containers;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using MongoDB.Bson;
using Testcontainers.MongoDb;

namespace IPFees.Core.Tests.Fixture
{
    public class FeeCalculatorFixture : IAsyncDisposable
    {
        public ModuleRepository ModuleRepository { get; set; }
        public FeeRepository FeeRepository { get; set; }
        private readonly IContainer mongoContainer;
        private readonly string connectionString = string.Empty;

        public FeeCalculatorFixture()
        {
            // Start the test MongoDb instance            
            mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .WithPortBinding(27017, true)
                .WithEnvironment("MONGO_INITDB_DATABASE", "IPFeesTest")
                //.WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "root")
                //.WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "password")
                .Build();

            // Start the container
            mongoContainer.StartAsync().GetAwaiter().GetResult();

            // Use the dynamically assigned host port
            connectionString = $"mongodb://root:password@localhost:{mongoContainer.GetMappedPublicPort(27017)}/IPFeesTest";

            // Build database context based on the connection string
            var DbContext = new DataContext(connectionString);
            DbContext.ModuleCollection.DeleteMany(new BsonDocument());
            DbContext.FeeCollection.DeleteMany(new BsonDocument());
            ModuleRepository = new ModuleRepository(DbContext);
            FeeRepository = new FeeRepository(DbContext);
        }

        public async ValueTask DisposeAsync()
        {
            // Stop and clean up the container
            await mongoContainer.DisposeAsync();
        }
    }
}
