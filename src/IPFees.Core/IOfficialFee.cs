using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFFees.Core
{
    public interface IOfficialFee
    {
        Task<OfficialFeeResult> GetVariables(string JurisdictionName);
        Task<OfficialFeeResult> Calculate(string JurisdictionName, IList<IPFValue> Vars);
    }
}