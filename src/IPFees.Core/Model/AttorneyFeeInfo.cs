using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record AttorneyFeeInfo(JurisdictionAttorneyFeeLevel FeeLevel, int Amount, string Currency);
}