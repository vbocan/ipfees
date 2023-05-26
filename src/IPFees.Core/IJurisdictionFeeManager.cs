using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Core
{
    public interface IJurisdictionFeeManager
    {
        (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<string> JurisdictionNames);
        Task<IEnumerable<FeeResult>> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues);
    }
}
