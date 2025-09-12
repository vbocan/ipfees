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
        private readonly string connectionString = "mongodb://root:pA$$w0rd@ipfees-mongodb:27017/IPFees?authSource=admin&retryWrites=true";
        private readonly string ExchangeApiKey = "1234567890"; // Get an actual API key from https://www.exchangerate-api.com/

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
