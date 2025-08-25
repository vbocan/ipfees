using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using MongoDB.Bson;

namespace IPFees.Core.Tests.Fixture
{
    public class CoreFixture : IDisposable
    {
        public ExchangeRateFetcher CurrencyConverter { get; set; }
        public FeeRepository FeeRepository { get; set; }
        public JurisdictionRepository JurisdictionRepository { get; set; }
        public ModuleRepository ModuleRepository { get; set; }
        public SettingsRepository SettingsRepository { get; set; }
        public DataContext DbContext { get; private set; }        
        private readonly string connectionString = "mongodb://admin:15rRDQ4xw1RLVtz@db.dataman.ro:27017/IPFeesTest?authSource=admin&retryWrites=true";
        private readonly string ExchangeApiKey = "2e0b92d2e214bd2d6b1cd895";

        public CoreFixture()
        {
            // Build database context based on the connection string
            DbContext = new DataContext(connectionString);
            DbContext.ModuleCollection.DeleteMany(new BsonDocument());
            DbContext.FeeCollection.DeleteMany(new BsonDocument());
            DbContext.JurisdictionCollection.DeleteMany(new BsonDocument());
            DbContext.ServiceFeesCollection.DeleteMany(new BsonDocument());

            CurrencyConverter = new ExchangeRateFetcher(ExchangeApiKey);
            FeeRepository = new FeeRepository(DbContext);
            JurisdictionRepository = new JurisdictionRepository(DbContext);
            ModuleRepository = new ModuleRepository(DbContext);
            SettingsRepository = new SettingsRepository(DbContext);
        }

        public void Dispose()
        {
        }
    }
}
