using IPFees.Core.Data;
using IPFees.Core.Enum;

namespace IPFees.Core.Repository
{
    public interface IKeyValueRepository
    {
        Task<DbResult> SetCategoryWeightAsync(string Category, int Weight);
        Task<int> GetCategoryWeightAsync(string Category);
        Task<DbResult> SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel feeLevel, int Amount, string Currency);
        Task<(int, string)> GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel feeLevel);
    }
}