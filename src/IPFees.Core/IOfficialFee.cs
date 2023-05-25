using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFees.Core
{
    public interface IOfficialFee
    {
        FeeResult GetInputs(Guid FeeId);
        (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<Guid> FeeIds);

        FeeResult Calculate(Guid FeeId, IList<IPFValue> InputValues);
        IEnumerable<FeeResult> Calculate(IEnumerable<Guid> FeeIds, IList<IPFValue> InputValues);        
    }
}