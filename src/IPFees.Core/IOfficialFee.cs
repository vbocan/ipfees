using IPFees.Evaluator;
using IPFFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFFees.Core
{
    public interface IOfficialFee
    {
        Task<CalculationResultBase> Calculate(string JurisdictionName, IList<IPFValue> Vars);
    }
}