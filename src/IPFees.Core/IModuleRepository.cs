using IPFees.Core.Data;

namespace IPFees.Core
{
    public interface IModuleRepository
    {
        Task<DbResult> AddModuleAsync(string Name);
        Task<DbResult> SetModuleNameAsync(Guid Id, string Name);
        Task<DbResult> SetModuleDescriptionAsync(Guid Id, string Description);
        Task<DbResult> SetModuleSourceCodeAsync(Guid Id, string SourceCode);
        Task<DbResult> RemoveModuleAsync(Guid Id);
        Task<IEnumerable<ModuleInfo>> GetModules();
        Task<ModuleInfo> GetModuleById(Guid Id);
    }
}