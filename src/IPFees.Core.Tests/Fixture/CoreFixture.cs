using IPFees.Calculator;
using IPFees.Core;
using IPFees.Core.Data;
using IPFees.Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IPFees.Core.Tests.Fixture
{
    public class CoreFixture : IDisposable
    {
        public DataContext DbContext { get; private set; }        
        private readonly string connectionString = "mongodb+srv://abdroot:Test123@cluster0.dusbo.mongodb.net/CoreTest?retryWrites=true&w=majority";

        public CoreFixture()
        {
            // Build database context based on the connection string
            DbContext = new DataContext(connectionString);
            DbContext.ModuleCollection.DeleteMany(new BsonDocument());
            DbContext.JurisdictionCollection.DeleteMany(new BsonDocument());
            DbContext.SettingsCollection.DeleteMany(new BsonDocument());            
        }

        public void Dispose()
        {
        }
    }
}
