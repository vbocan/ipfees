using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IPFees.Core.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DataContext context;
        public SettingsRepository(DataContext context)
        {
            this.context = context;
        }        

        #region Service Fees Level Settings
        public async Task<DbResult> SetServiceFeeAsync(ServiceFeeLevel FeeLevel, double Amount, string Currency)
        {
            var res = await context.ServiceFeesCollection.UpdateOneAsync(r => r.FeeLevel.Equals(FeeLevel),
                Builders<ServiceFeesDoc>
                .Update
                .Set(r => r.Amount, Amount)
                .Set(r => r.Currency, Currency),
                new UpdateOptions { IsUpsert = true }
                );
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        public async Task<IEnumerable<ServiceFeeInfo>> GetServiceFeesAsync()
        {
            var dbObjs = await context.ServiceFeesCollection.FindAsync(new BsonDocument());
            return dbObjs.ToList().Adapt<IEnumerable<ServiceFeeInfo>>();
        }

        public async Task<ServiceFeeInfo> GetServiceFeeAsync(ServiceFeeLevel FeeLevel)
        {
            var filter = Builders<ServiceFeesDoc>.Filter.Eq(m => m.FeeLevel, FeeLevel);
            var dbObjs = (await context.ServiceFeesCollection.FindAsync(filter)).FirstOrDefaultAsync().Result;
            if (dbObjs != null)
            {
                return dbObjs.Adapt<ServiceFeeInfo>();
            }
            else
            {
                return new ServiceFeeInfo(FeeLevel, 0, string.Empty);
            }
        }
        #endregion
    }
}