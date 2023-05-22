using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Xml.Linq;
using static MongoDB.Driver.WriteConcern;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<DbResult> SetModuleGroupAsync(string GroupName, string GroupDescription)
        {
            var filter = Builders<SettingsDoc>.Filter.Empty;
            var update = Builders<SettingsDoc>.Update.AddToSet(f => f.ModuleGroups, new ModuleGroup(GroupName, GroupDescription));
            var res = await context.SettingsCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        public async Task<(string?, int)> GetModuleGroupAsync(string GroupName)
        {
            var filter = Builders<SettingsDoc>.Filter.Empty;            
            var res = (await context.SettingsCollection.FindAsync(filter)).FirstOrDefault();
            var GroupDescription = res.ModuleGroups.FirstOrDefault(w=>w.GroupName.Equals(GroupName))?.GroupDescription;            
            var GroupIndex = res.ModuleGroups.FindIndex(w => w.GroupName.Equals(GroupName));
            return (GroupDescription, GroupIndex);
        }

        public async Task MoveModuleGroupDownAsync(string GroupName)
        {
            var filter = Builders<SettingsDoc>.Filter.Empty;
            var res = (await context.SettingsCollection.FindAsync(filter)).FirstOrDefault();
            MoveGroupDown(res.ModuleGroups, GroupName);
        }

        public async Task MoveModuleGroupUpAsync(string GroupName)
        {
            var filter = Builders<SettingsDoc>.Filter.Empty;
            var res = (await context.SettingsCollection.FindAsync(filter)).FirstOrDefault();
            MoveGroupUp(res.ModuleGroups, GroupName);
        }
        #endregion

        #region Helpers
        private void MoveGroupDown(List<ModuleGroup> items, string GroupName)
        {
            // Find the index of the record with the specified group name
            int index = items.FindIndex(record => record.GroupName.Equals(GroupName));
            // If the record was found and it's not already the last item in the list
            if (index != -1 && index < items.Count - 1)
            {
                // Swap the record with the next one
                (items[index + 1], items[index]) = (items[index], items[index + 1]);
            }
        }

        private void MoveGroupUp(List<ModuleGroup> items, string GroupName)
        {
            // Find the index of the record with the specified group name
            int index = items.FindIndex(record => record.GroupName.Equals(GroupName));
            // If the record was found and it's not already the first item in the list
            if (index != -1 && index > 0)
            {
                // Swap the record with the previous one
                (items[index - 1], items[index]) = (items[index], items[index - 1]);
            }
        }
        #endregion

        #region Attorney Fees Level Settings
        public async Task<DbResult> SetAttorneyFeeLevelAsync(JurisdictionAttorneyFeeLevel Level, double Amount, string Currency)
        {
            var filter = Builders<SettingsDoc>.Filter.Empty;
            var update = Builders<SettingsDoc>.Update.AddToSet(f => f.AttorneyFeeLevels, new AttorneyFee(Level, Amount, Currency));
            var res = await context.SettingsCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        public async Task<(double, string?)> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel Level)
        {
            var filter = Builders<SettingsDoc>.Filter.Empty;
            var res = (await context.SettingsCollection.FindAsync(filter)).FirstOrDefault();
            var af = res.AttorneyFeeLevels.FirstOrDefault(w => w.Level.Equals(Level));            
            return (af?.Amount ?? 0.0, af?.Currency ?? string.Empty);
        }
        #endregion
    }
}