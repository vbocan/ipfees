namespace IPFees.Core
{
    public record JurisdictionInfo(Guid Id, JurisdictionCategory Category, JurisdictionAttorneyFeeLevel AttorneyFeeLevel, string Name, string Description, string SourceCode, IEnumerable<Guid> ReferencedModules, DateTime LastUpdatedOn);
}