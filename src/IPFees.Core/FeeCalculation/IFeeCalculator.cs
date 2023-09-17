using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Core.FeeCalculation
{
    public interface IFeeCalculator
    {
        FeeResult GetInputs(Guid FeeId);
        FeeResult Calculate(Guid FeeId, IList<IPFValue> InputValues);
    }
}