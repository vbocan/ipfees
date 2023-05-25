using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record AttorneyFeeInfo(AttorneyFeeLevel FeeLevel, double Amount, string Currency);
}