using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;

namespace IPFees.Core.Repository
{
    public interface ISettingsRepository
    {
        Task<DbResult> SetServiceFeeAsync(ServiceFeeLevel FeeLevel, double Amount, string Currency);
        Task<IEnumerable<ServiceFeeInfo>> GetServiceFeesAsync();
        Task<ServiceFeeInfo> GetServiceFeeAsync(ServiceFeeLevel FeeLevel);
    }
}