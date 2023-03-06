using IPFFees.Core.Data;

namespace IPFFees.Core
{
    public interface IModule
    {
        Task<DbResult> AddAsync(string name, string description, string code);
        Task<DbResult> RemoveAsync(string name);
        IEnumerable<ModuleInfo> GetModules();
    }
}