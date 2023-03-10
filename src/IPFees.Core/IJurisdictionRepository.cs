using IPFees.Evaluator;
using IPFFees.Core.Data;

namespace IPFFees.Core
{
    public interface IJurisdictionRepository
    {
        Task<DbResult> AddJurisdictionAsync(string JurisdictionName);
        Task<DbResult> SetJurisdictionSourceCodeAsync(string JurisdictionName, string SourceCode);
        Task<DbResult> RemoveJurisdictionAsync(string JurisdictionName);
        Task<IEnumerable<JurisdictionInfo>> GetJurisdictions();
        Task<JurisdictionInfo> GetJurisdictionByName(string JurisdictionName);
        Task<DbResult> SetReferencedModules(string JurisdictionName, string[] ModuleNames);        
    }
}