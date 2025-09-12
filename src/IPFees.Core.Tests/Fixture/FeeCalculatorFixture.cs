using IPFees.Calculator;
using IPFees.Core.Data;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IPFees.Core.Tests.Fixture
{
    public class FeeCalculatorFixture : IDisposable
    {
        public ModuleRepository ModuleRepository { get; set; }
        public FeeRepository FeeRepository { get; set; }
        private readonly string connectionString = "mongodb://root:pA$$w0rd@ipfees-mongodb:27017/IPFeesTest?authSource=admin&retryWrites=true";

        public FeeCalculatorFixture()
        {
            // Build database context based on the connection string
            var DbContext = new DataContext(connectionString);
            DbContext.ModuleCollection.DeleteMany(new BsonDocument());
            DbContext.FeeCollection.DeleteMany(new BsonDocument());
            ModuleRepository = new ModuleRepository(DbContext);
            FeeRepository = new FeeRepository(DbContext);
        }

        public void Dispose()
        {
        }
    }
}
