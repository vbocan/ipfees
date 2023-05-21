using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.Model;

namespace IPFees.Core.Repository
{
    public interface IJurisdictionRepository
    {
        Task<DbResult> AddJurisdictionAsync(string Name);
        Task<DbResult> SetJurisdictionCategoryAsync(Guid Id, JurisdictionCategory Category);
        Task<DbResult> SetJurisdictionAttorneyFeeLevelAsync(Guid Id, JurisdictionAttorneyFeeLevel AttorneyFeeLevel);
        Task<DbResult> SetJurisdictionNameAsync(Guid Id, string Name);
        Task<DbResult> SetJurisdictionDescriptionAsync(Guid Id, string Description);
        Task<DbResult> SetJurisdictionSourceCodeAsync(Guid Id, string SourceCode);
        Task<DbResult> RemoveJurisdictionAsync(Guid Id);
        Task<IEnumerable<JurisdictionInfo>> GetJurisdictions();
        Task<JurisdictionInfo> GetJurisdictionById(Guid Id);
        Task<DbResult> SetReferencedModules(Guid Id, IList<Guid> ModuleIds);
    }
}