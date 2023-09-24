using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record JurisdictionInfo(Guid Id, string Name, string Description, ServiceFeeLevel ServiceFeeLevel, DateTime LastUpdatedOn);
}