using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record JurisdictionInfo(Guid Id, string Name, string Description, string JurisdictionName, ServiceFeeLevel ServiceFeeLevel, DateTime LastUpdatedOn);
}