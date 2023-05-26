using IPFees.Core.Enum;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFees.Evaluator;
using IPFees.Parser;
using System;

namespace IPFees.Core
{
    public class JurisdictionFeeManager : IJurisdictionFeeManager
    {
        private readonly IFeeCalculator feeCalculator;
        private readonly IFeeRepository feeRepository;        

        public JurisdictionFeeManager(IFeeCalculator feeCalculator, IFeeRepository feeRepository)
        {
            this.feeCalculator = feeCalculator;
            this.feeRepository = feeRepository;            
        }

        public (IEnumerable<DslInput>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<string> JurisdictionNames)
        {
            var Inputs = new List<DslInput>();
            var Errors = new List<FeeResultFail>();
            foreach(var jur in JurisdictionNames)
            {
                var fees = GetFeesForJurisdiction(jur);
                foreach (var f in fees)
                {
                    var inp = feeCalculator.GetInputs(f.Id);
                    if (inp is FeeResultFail)
                    {
                        Errors.Add(inp as FeeResultFail);
                    }
                    else
                    {
                        var fps = inp as FeeResultParse;
                        Inputs.AddRange(fps.FeeInputs);
                    }
                }
            }
            var DedupedInputs = Inputs.DistinctBy(d => d.Name);
            return (DedupedInputs, Errors);
        }

        public IEnumerable<FeeResult> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues)
        {
            var Results = new List<FeeResult>();
            foreach (var jur in JurisdictionNames)
            {
                var fees = GetFeesForJurisdiction(jur);
                foreach(var f in fees)
                {
                    // TODO:
                    // Evaluate the current fee
                    // var res = feeCalculator.Calculate(f.Id, InputValues);
                    // Strore result according to fee type (official fee, partner fee, attorney fee)
                    //Results.AddRange(res);
                }
            }
            return Results;
        }

        private IEnumerable<FeeInfo> GetFeesForJurisdiction(string JurisdictionName) => feeRepository.GetFees().Result.Where(w => w.JurisdictionName.Equals(JurisdictionName));
    }
}
