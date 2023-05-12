using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFees.Core
{
    public interface IOfficialFee
    {
        FeeResult GetInputs(Guid JurisdictionId);
        (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<Guid> JurisdictionIds);

        FeeResult Calculate(Guid JurisdictionId, IList<IPFValue> InputValues);
        IEnumerable<FeeResult> Calculate(IEnumerable<Guid> JurisdictionIds, IList<IPFValue> InputValues);        
    }
}