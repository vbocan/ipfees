using IPFFees.Core.Data;

namespace IPFFees.Core
{
    public interface IModuleRepository
    {
        Task<DbResult> AddModuleAsync(string ModuleName);
        Task<DbResult> SetModuleDescriptionAsync(string ModuleName, string Description);
        Task<DbResult> SetModuleSourceCodeAsync(string ModuleName, string SourceCode);
        Task<DbResult> RemoveModuleAsync(string ModuleName);
        IEnumerable<ModuleInfo> GetModules();
        ModuleInfo GetModuleByName(string ModuleName);
    }
}