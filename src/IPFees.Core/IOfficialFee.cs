using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFFees.Core
{
    public interface IOfficialFee
    {
        Task<OfficialFeeResult> GetVariables(Guid JurisdictionId);
        Task<OfficialFeeResult> Calculate(Guid JurisdictionId, IList<IPFValue> Vars);
    }
}