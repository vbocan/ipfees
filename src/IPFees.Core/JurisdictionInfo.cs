namespace IPFFees.Core
{
    public record JurisdictionInfo(Guid Id, string Name, string Description, string SourceCode, Guid[] ReferencedModules, DateTime LastUpdatedOn);
}