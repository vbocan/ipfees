using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record AttorneyFeeInfo(JurisdictionAttorneyFeeLevel FeeLevel, double Amount, string Currency);
}