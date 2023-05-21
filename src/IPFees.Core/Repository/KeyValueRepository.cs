using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.WriteConcern;

namespace IPFees.Core.Repository
{
    public class KeyValueRepository : IKeyValueRepository
    {
        private readonly DataContext context;
        private const string CategoryDescriptionPrefix = "CATEGORYDESCRIPTION";
        private const string CategoryWeightPrefix = "CATEGORYWEIGHT";        
        private const string AttorneyFeeAmountPrefix = "ATTORNEYFEEAMOUNT";
        private const string AttorneyFeeCurrencyPrefix = "ATTORNEYFEECURRENCY";

        public KeyValueRepository(DataContext context)
        {
            this.context = context;
        }

        #region Category Settings
        public async Task<DbResult> SetCategoryAsync(string Name, int Weight, string Description)
        {
            var res1 = await SetKeyAsync($"{CategoryWeightPrefix}_{Name}", Weight);
            if (!res1.Success) return res1;
            var res2 = await SetKeyAsync($"{CategoryDescriptionPrefix}_{Name}", Description);
            if (!res2.Success) return res2;            
            return DbResult.Succeed();            
        }
        public async Task<(int, string)> GetCategoryAsync(string Name)
        {
            var obj = await GetKeyAsync($"{CategoryWeightPrefix}_{Name}");
            int Weight = Convert.ToInt32(obj ?? 0);
            var Description = await GetKeyAsync($"{CategoryDescriptionPrefix}_{Name}") as string;
            return (Weight, Description ?? string.Empty);
        }
        #endregion

        #region Attorney Fee Settings
        public async Task<DbResult> SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel feeLevel, int Amount, string Currency)
        {
            var res1 = await SetKeyAsync($"{AttorneyFeeAmountPrefix}_{feeLevel}", Amount);
            if (!res1.Success) return res1;
            var res2 = await SetKeyAsync($"{AttorneyFeeCurrencyPrefix}_{feeLevel}", Currency);
            if (!res2.Success) return res2;
            return DbResult.Succeed();
        }

        public async Task<(int, string)> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel feeLevel)
        {
            var obj = await GetKeyAsync($"{AttorneyFeeAmountPrefix}_{feeLevel}");
            int Amount = Convert.ToInt32(obj ?? 0);
            var Currency = await GetKeyAsync($"{AttorneyFeeCurrencyPrefix}_{feeLevel}") as string;
            return (Amount, Currency ?? string.Empty);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Set key to specified value
        /// </summary>        
        /// <param name="Key">Key</param>
        /// <param name="Value">Value</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        private async Task<DbResult> SetKeyAsync(string Key, object Value)
        {
            var res = await context.KeyValueCollection.UpdateOneAsync(r => r.Key.Equals(Key),
                Builders<KeyValueDoc>
                .Update
                .Set(r => r.Value, Value), new UpdateOptions { IsUpsert = true });
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Get the value of a key
        /// </summary>
        /// <param name="Key">Key</param>
        /// <returns>Value or null if the key has not been previously set</returns>
        private async Task<object?> GetKeyAsync(string Key)
        {
            var res = (await context.KeyValueCollection.FindAsync(r => r.Key.Equals(Key))).SingleOrDefault();
            if (res != null)
            {
                return res.Value;
            }
            else { return null; }
        }
        #endregion
    }
}