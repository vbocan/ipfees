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
        private readonly ISettingsRepository settingsRepository;
        private readonly IJurisdictionRepository jurisdictionRepository;

        public JurisdictionFeeManager(IFeeCalculator feeCalculator, IFeeRepository feeRepository, JurisdictionRepository jurisdictionRepository, SettingsRepository settingsRepository)
        {
            this.feeCalculator = feeCalculator;
            this.feeRepository = feeRepository;
            this.jurisdictionRepository = jurisdictionRepository;
            this.settingsRepository = settingsRepository;
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

        public async Task<IEnumerable<FeeResult>> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues)
        {
            double OfficialAmount = 0;
            double PartnerAmount = 0;
            double AttorneyAmount = 0;
            var Errors = new List<FeeResultFail>();

            var Results = new List<FeeResult>();
            foreach (var jn in JurisdictionNames)
            {
                #region Attorney Fee
                var jur = await jurisdictionRepository.GetJurisdictionByName(jn);                
                var AttorneyFee = await settingsRepository.GetAttorneyFeeAsync(jur.AttorneyFeeLevel);
                AttorneyAmount += AttorneyFee.Amount;
                #endregion

                #region Official and Partner Fees
                var fees = GetFeesForJurisdiction(jn);
                foreach(var f in fees)
                {                                        
                    var res = feeCalculator.Calculate(f.Id, InputValues);
                    if (res is FeeResultFail)
                    {
                        Errors.Add(res as FeeResultFail);
                    }
                    else
                    {
                        var frc = res as FeeResultCalculation;
                        switch (f.Category)
                        {
                            case FeeCategory.OfficialFees:
                                OfficialAmount += ; //TODO: Add the actual amount
                                break;
                            case FeeCategory.PartnerFees:
                                PartnerAmount += ; //TODO: Add the actual amount
                                break;
                        }
                    }                    
                    // TODO: Take care of the currency! There may be multiple resulting currencies!
                }
                #endregion
            }
            return Results;
        }

        private IEnumerable<FeeInfo> GetFeesForJurisdiction(string JurisdictionName) => feeRepository.GetFees().Result.Where(w => w.JurisdictionName.Equals(JurisdictionName));
    }
}
