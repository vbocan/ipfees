using IPFees.Core.Repository;
using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Core
{
    public class JurisdictionFeeManager
    {
        private readonly IFeeCalculator feeCalculator;
        private readonly IFeeRepository feeRepository;

        public JurisdictionFeeManager(IFeeCalculator feeCalculator, IFeeRepository feeRepository)
        {
            this.feeCalculator = feeCalculator;
            this.feeRepository = feeRepository;
        }

        (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<string> JurisdictionNames)
        {
            var Inputs = new List<DslInput>();
            var Errors = new List<FeeResultFail>();
            foreach(var jur in JurisdictionNames)
            {
                var fees = GetFeesForJurisdiction(jur);
                var (x, y) = feeCalculator.GetConsolidatedInputs(fees);
                Inputs.AddRange(x);
                Errors.AddRange(y);                 
            }
            return (Inputs, Errors);
        }

        IEnumerable<FeeResult> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues)
        {
            var Results = new List<FeeResult>();
            foreach (var jur in JurisdictionNames)
            {
                var fees = GetFeesForJurisdiction(jur);
                var res = feeCalculator.Calculate(fees, InputValues);
                Results.AddRange(res);
            }
            return Results;
        }

        private IEnumerable<Guid> GetFeesForJurisdiction(string JurisdictionName) => feeRepository.GetFees().Result.Where(w => w.JurisdictionName.Equals(JurisdictionName)).Select(s => s.Id);
    }
}
