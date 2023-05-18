using IPFees.Core.Data;
using IPFees.Core.Models;

namespace IPFees.Core
{
    public interface IJurisdictionRepository
    {
        Task<DbResult> AddJurisdictionAsync(string Name);
        Task<DbResult> SetJurisdictionCategoryAsync(Guid Id, JurisdictionCategory Category);        
        Task<DbResult> SetJurisdictionNameAsync(Guid Id, string Name);
        Task<DbResult> SetJurisdictionDescriptionAsync(Guid Id, string Description);
        Task<DbResult> SetJurisdictionSourceCodeAsync(Guid Id, string SourceCode);
        Task<DbResult> RemoveJurisdictionAsync(Guid Id);
        Task<IEnumerable<JurisdictionInfo>> GetJurisdictions();
        Task<JurisdictionInfo> GetJurisdictionById(Guid Id);
        Task<DbResult> SetReferencedModules(Guid Id, IList<Guid> ModuleIds);
    }
}