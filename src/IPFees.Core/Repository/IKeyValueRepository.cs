using IPFees.Core.Data;
using IPFees.Core.Enum;

namespace IPFees.Core.Repository
{
    public interface IKeyValueRepository
    {
        Task<DbResult> SetCategoryAsync(string Name, int Weight, string Description);
        Task<(int, string)> GetCategoryAsync(string Name);
        Task<DbResult> SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel feeLevel, int Amount, string Currency);
        Task<(int, string)> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel feeLevel);
    }
}