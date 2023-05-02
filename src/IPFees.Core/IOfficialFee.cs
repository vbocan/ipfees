using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFFees.Core
{
    public interface IOfficialFee
    {
        Task<FeeResult> GetInputs(Guid JurisdictionId);        
        Task<(IEnumerable<DslInput>, IEnumerable<FeeResultFail>)> GetConsolidatedInputs(IEnumerable<Guid> JurisdictionIds);

        Task<FeeResult> Calculate(Guid JurisdictionId, IList<IPFValue> InputValues);
        IAsyncEnumerable<FeeResult> Calculate(IEnumerable<Guid> JurisdictionIds, IList<IPFValue> InputValues);        
    }
}