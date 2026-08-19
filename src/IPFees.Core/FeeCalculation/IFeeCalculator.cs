using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFees.Core.FeeCalculation
{
    public interface IFeeCalculator
    {
        FeeResult GetInputs(Guid FeeId);
        FeeResult Calculate(Guid FeeId, IList<IPFValue> InputValues);

        /// <summary>
        /// Run the fee's static verification directives (completeness, monotonicity).
        /// </summary>
        FeeResult Verify(Guid FeeId);

        /// <summary>
        /// Compute the fee and record why each amount arose, optionally with counterfactuals.
        /// </summary>
        FeeResult Explain(Guid FeeId, IList<IPFValue> InputValues, bool IncludeAlternatives = false);
    }
}