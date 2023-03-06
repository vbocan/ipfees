using IPFFees.Core.Data;

namespace IPFFees.Core
{
    public interface IModule
    {
        Task<DbResult> AddModuleAsync(string name, string description, string code);
        Task<DbResult> EditModuleAsync(string name, string description, string code);
        Task<DbResult> RemoveModuleAsync(string name);
        IEnumerable<ModuleInfo> GetModules();
    }
}