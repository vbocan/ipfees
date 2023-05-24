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

        #region Category Settings
        public async Task<DbResult> SetModuleGroupAsync(string GroupName, string GroupDescription, int GroupWeight)
        {
            var res = await context.ModuleGroupsCollection.UpdateOneAsync(r => r.GroupName.Equals(GroupName),
                Builders<ModuleGroupsDoc>
                .Update
                .Set(r => r.GroupName, GroupName)
                .Set(r => r.GroupDescription, GroupDescription)
                .Set(r => r.GroupWeight, GroupWeight),
                new UpdateOptions { IsUpsert = true }
                );
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        public async Task<IEnumerable<ModuleGroupInfo>> GetModuleGroupsAsync()
        {
            var dbObjs = await context.ModuleGroupsCollection.FindAsync(new BsonDocument());
            return dbObjs.ToList().Adapt<IEnumerable<ModuleGroupInfo>>();
        }

        public async Task<ModuleGroupInfo> GetModuleGroupAsync(string GroupName)
        {
            var filter = Builders<ModuleGroupsDoc>.Filter.Eq(m => m.GroupName, GroupName);
            var dbObjs = (await context.ModuleGroupsCollection.FindAsync(filter)).FirstOrDefaultAsync().Result;
            if (dbObjs != null)
            {
                return dbObjs.Adapt<ModuleGroupInfo>();
            }
            else
            {
                return new ModuleGroupInfo(GroupName, string.Empty, -1);
            }
        }        
        #endregion        

        #region Attorney Fees Level Settings
        public async Task<DbResult> SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel FeeLevel, double Amount, string Currency)
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

        public async Task<AttorneyFeeInfo> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel FeeLevel)
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