using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record ServiceFeeInfo(ServiceFeeLevel FeeLevel, decimal Amount, string Currency);
}