using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;

namespace IPFees.Core.Repository
{
    public interface IFeeRepository
    {
        Task<DbResult> AddFeeAsync(string Name);
        Task<DbResult> SetFeeCategoryAsync(Guid Id, FeeCategory Category);
        Task<DbResult> SetAttorneyFeeLevelAsync(Guid Id, AttorneyFeeLevel AttorneyFeeLevel);
        Task<DbResult> SetFeeNameAsync(Guid Id, string Name);
        Task<DbResult> SetFeeDescriptionAsync(Guid Id, string Description);
        Task<DbResult> SetFeeSourceCodeAsync(Guid Id, string SourceCode);
        Task<DbResult> RemoveFeeAsync(Guid Id);
        Task<IEnumerable<FeeInfo>> GetFees();
        Task<FeeInfo> GetFeeById(Guid Id);
        Task<DbResult> SetReferencedModules(Guid Id, IList<Guid> ModuleIds);
    }
}