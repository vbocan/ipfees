using IPFees.Calculator;
using IPFFees.Core;
using IPFFees.Core.Data;
using IPFFees.Core.Models;
using MongoDB.Driver;

namespace IPFees.Core.Tests.Fixture
{
    public class OfficialFeeFixture : IDisposable
    {
        public DataContext DbContext { get; private set; }
        private readonly string connectionString = "mongodb+srv://abdroot:Test123@cluster0.dusbo.mongodb.net/OfficialFeeTest?retryWrites=true&w=majority";

        public OfficialFeeFixture()
        {
            // Build database context based on the connection string
            DbContext = new DataContext(connectionString);
            DbContext.DropDatabase();
        }

        public void Dispose()
        {            
        }
    }
}
