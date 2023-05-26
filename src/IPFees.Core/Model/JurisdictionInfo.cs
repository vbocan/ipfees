using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record JurisdictionInfo(Guid Id, string Name, string Description, string JurisdictionName, AttorneyFeeLevel AttorneyFeeLevel, DateTime LastUpdatedOn);
}