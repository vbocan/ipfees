using IPFFees.Core.Data;

namespace IPFFees.Core
{
    public interface IJurisdiction
    {
        Task<DbResult> AddJurisdictionAsync(string JurisdictionName);
        Task<DbResult> SetJurisdictionSourceCodeAsync(string JurisdictionName, string SourceCode);
        Task<DbResult> RemoveJurisdictionAsync(string JurisdictionName);
        IEnumerable<JurisdictionInfo> GetJurisdictions();
        JurisdictionInfo GetJurisdictionByName(string JurisdictionName);
        Task<DbResult> SetReferencedModules(string JurisdictionName, string[] ModuleNames);
    }
}