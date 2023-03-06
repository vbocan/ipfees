using IPFees.Core.Helpers;
using IPFFees.Core.Data;
using IPFFees.Core.Models;
using MongoDB.Driver;

namespace IPFees.Core.Tests.Fixture
{
    public class CoreFixture : IDisposable
    {
        public DataContext DbContext { get; private set; }
        public IDateTimeHelper DateHelper { get; private set; }
        public IMongoCollection<ModuleDoc> ModuleCollection { get; private set; }
        private readonly string connectionString = "mongodb+srv://abdroot:Test123@cluster0.dusbo.mongodb.net/IPFees?retryWrites=true&w=majority";

        public CoreFixture()
        {
            // Build database context based on the connection string
            DbContext = new DataContext(connectionString);
            ModuleCollection = DbContext.ModuleCollection;
            DateHelper = new DateTimeHelper();
            DbContext.DropDatabase();
        }

        public void Dispose()
        {
            DbContext.DropDatabase();
        }
    }
}
