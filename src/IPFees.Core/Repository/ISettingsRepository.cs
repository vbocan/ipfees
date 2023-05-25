using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;

namespace IPFees.Core.Repository
{
    public interface ISettingsRepository
    {
        Task<DbResult> SetAttorneyFeeAsync(AttorneyFeeLevel FeeLevel, double Amount, string Currency);
        Task<IEnumerable<AttorneyFeeInfo>> GetAttorneyFeesAsync();
        Task<AttorneyFeeInfo> GetAttorneyFeeAsync(AttorneyFeeLevel FeeLevel);
    }
}