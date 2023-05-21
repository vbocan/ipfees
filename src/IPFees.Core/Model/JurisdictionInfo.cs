using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record JurisdictionInfo(Guid Id, JurisdictionCategory Category, JurisdictionAttorneyFeeLevel AttorneyFeeLevel, string Name, string Description, string SourceCode, IEnumerable<Guid> ReferencedModules, DateTime LastUpdatedOn);
}