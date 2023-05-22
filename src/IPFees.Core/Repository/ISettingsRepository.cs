using IPFees.Core.Data;
using IPFees.Core.Enum;

namespace IPFees.Core.Repository
{
    public interface ISettingsRepository
    {
        Task<DbResult> SetModuleGroupAsync(string GroupName, string GroupDescription);
        Task<(string, int)> GetModuleGroupAsync(string GroupName);
        Task MoveModuleGroupDownAsync(string GroupName);
        Task MoveModuleGroupUpAsync(string GroupName);
        Task<DbResult> SetAttorneyFeeLevelAsync(JurisdictionAttorneyFeeLevel Level, double Amount, string Currency);
        Task<(double, string)> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel Level);
    }
}