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

        #region Attorney Fees Level Settings
        public async Task<DbResult> SetAttorneyFeeAsync(AttorneyFeeLevel FeeLevel, double Amount, string Currency)
        {
            var res = await context.AttorneyFeesCollection.UpdateOneAsync(r => r.FeeLevel.Equals(FeeLevel),
                Builders<AttorneyFeesDoc>
                .Update
                .Set(r => r.Amount, Amount)
                .Set(r => r.Currency, Currency),
                new UpdateOptions { IsUpsert = true }
                );
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        public async Task<IEnumerable<AttorneyFeeInfo>> GetAttorneyFeesAsync()
        {
            var dbObjs = await context.AttorneyFeesCollection.FindAsync(new BsonDocument());
            return dbObjs.ToList().Adapt<IEnumerable<AttorneyFeeInfo>>();
        }

        public async Task<AttorneyFeeInfo> GetAttorneyFeeAsync(AttorneyFeeLevel FeeLevel)
        {
            var filter = Builders<AttorneyFeesDoc>.Filter.Eq(m => m.FeeLevel, FeeLevel);
            var dbObjs = (await context.AttorneyFeesCollection.FindAsync(filter)).FirstOrDefaultAsync().Result;
            if (dbObjs != null)
            {
                return dbObjs.Adapt<AttorneyFeeInfo>();
            }
            else
            {
                return new AttorneyFeeInfo(FeeLevel, 0, string.Empty);
            }
        }
        #endregion
    }
}