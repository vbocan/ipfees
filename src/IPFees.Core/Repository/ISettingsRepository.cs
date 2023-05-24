using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;

namespace IPFees.Core.Repository
{
    public interface ISettingsRepository
    {
        Task<DbResult> SetModuleGroupAsync(string GroupName, string GroupDescription, int GroupWeight);
        Task<IEnumerable<ModuleGroupInfo>> GetModuleGroupsAsync();
        Task<ModuleGroupInfo> GetModuleGroupAsync(string GroupName);
        Task<DbResult> SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel FeeLevel, double Amount, string Currency);
        Task<IEnumerable<AttorneyFeeInfo>> GetAttorneyFeesAsync();
        Task<AttorneyFeeInfo> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel FeeLevel);
    }
}