namespace IPFFees.Core
{
    public record JurisdictionInfo(string Name, string Description, string SourceCode, string[] ReferencedModules, DateTime LastUpdatedOn);
}