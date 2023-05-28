using IPFees.Core.Data;
using IPFees.Core.Model;

namespace IPFees.Core.Repository
{
    public interface IModuleRepository
    {
        Task<DbResult> AddModuleAsync(string Name);
        Task<DbResult> SetModuleNameAsync(Guid Id, string Name);
        Task<DbResult> SetModuleDescriptionAsync(Guid Id, string Description);
        Task<DbResult> SetModuleSourceCodeAsync(Guid Id, string SourceCode);
        Task<DbResult> SetModuleAutoRunStatusAsync(Guid Id, bool AutoRun);
        Task<DbResult> RemoveModuleAsync(Guid Id);
        Task<IEnumerable<ModuleInfo>> GetModules();
        Task<ModuleInfo> GetModuleById(Guid Id);
    }
}