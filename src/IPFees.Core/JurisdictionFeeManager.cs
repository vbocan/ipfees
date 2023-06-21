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

        public JurisdictionFeeManager(IFeeCalculator feeCalculator, IFeeRepository feeRepository, IJurisdictionRepository jurisdictionRepository, ISettingsRepository settingsRepository)
        {
            this.feeCalculator = feeCalculator;
            this.feeRepository = feeRepository;
            this.jurisdictionRepository = jurisdictionRepository;
            this.settingsRepository = settingsRepository;
        }
        
        public (IEnumerable<DslInput>, IEnumerable<DslGroup>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<string> JurisdictionNames)
        {
            var Inputs = new List<DslInput>();
            var Groups = new List<DslGroup>();
            var Errors = new List<FeeResultFail>();
            foreach (var jur in JurisdictionNames)
            {
                var fees = GetFeeDefinitionForJurisdiction(jur);
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
                        Groups.AddRange(fps.FeeGroups);
                    }
                }
            }
            var DedupedInputs = Inputs.DistinctBy(d => d.Name);
            var DedupedGroups = Groups.DistinctBy(d => d.Name);
            return (DedupedInputs, DedupedGroups, Errors);
        }

        /// <summary>
        /// Calculate the fees for several jurisdictions, as required by the end user (i.e. only the total amount is needed, and no calculation steps)
        /// </summary>
        /// <param name="JurisdictionNames">An enumeration of jurisdictions for which to perform the calculation</param>
        /// <param name="InputValues">Inputs needed by the calculation process. Obtain these inputs with a call to GetConsolidatedInputs.</param>
        /// <returns></returns>
        public async Task<TotalFeeInfo> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues)
        {
            /// There are three types of fees that are calculated for each jurisdiction:
            /// - Official fee - the amount paid to the government receiving the application
            /// - Partner fee - the amount paid to a partner in the respective jurisdiction, which does the translation and the actual application fileing work
            /// - Service fee - the amount paid to the partner which orchestrates the filing of the aplication, i.e. collaborates with the client for each individual jurisdiction
            /// Note: Each individual fee has its associated currency. At a later stage, the user shall convert the source currencies into whatever final target currency may be (usually EUR or USD).
            var OfficialFees = new List<FeeAmount>();
            var PartnerFees = new List<FeeAmount>();
            var ServiceFees = new List<FeeAmount>();
            var Errors = new List<FeeResultFail>();

            // Cycle through each required jurisdiction            
            foreach (var jn in JurisdictionNames)
            {
                // Calculate the associated service fee (the simplest of all fees)
                // Each jurisdiction has an associated service fee level (e.g. Level 1, 2, a.s.o.) and each level has an associated monetary value and currency
                #region Service Fee
                var jur = await jurisdictionRepository.GetJurisdictionByName(jn);
                var ServiceFee = await settingsRepository.GetServiceFeeAsync(jur.ServiceFeeLevel);
                ServiceFees.Add(new FeeAmount(ServiceFee.Amount, 0, ServiceFee.Currency, $"Service fee for jurisdiction '{jur.Name}'"));
                #endregion

                // Calculate the official and partner fees
                #region Official and Partner Fees
                // For the current jurisdiction, we need the associated fees defined in the system. These fees have different purposes (e.g. OfficialFee and PartnerFee) and we
                // need to discriminate between those.
                var FeeDefinitions = GetFeeDefinitionForJurisdiction(jn);
                // Let's tackle one fee definition at a time
                foreach (var fd in FeeDefinitions)
                {
                    // Perform the calculation on the current fee definition
                    var res = feeCalculator.Calculate(fd.Id, InputValues);
                    if (res is FeeResultFail)
                    {
                        // If the calculation has failed, accrue the errors in the list
                        Errors.Add(res as FeeResultFail);
                    }
                    else
                    {
                        // If the calculation has succeeded, store the result according to its type
                        var frc = res as FeeResultCalculation;
                        switch (fd.Category)
                        {
                            case FeeCategory.OfficialFees:
                                OfficialFees.Add(new FeeAmount(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty),
                                    $"Official fee for jurisdiction '{jur.Name}'"
                                    ));
                                break;
                            case FeeCategory.PartnerFees:
                                PartnerFees.Add(new FeeAmount(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty),
                                    $"Partner fee for jurisdiction '{jur.Name}'"
                                    ));
                                break;
                        }
                    }                    
                }
                #endregion
            }                        
            return new TotalFeeInfo { OfficialFees = OfficialFees, PartnerFees = PartnerFees, ServiceFees = ServiceFees, Errors = Errors };
        }

        private IEnumerable<FeeInfo> GetFeeDefinitionForJurisdiction(string JurisdictionName) => feeRepository.GetFees().Result.Where(w => w.JurisdictionName.Equals(JurisdictionName));
    }

    public record FeeAmount(double MandatoryAmount, double OptionalAmount, string Currency, string Description);
    public class TotalFeeInfo
    {
        public IList<FeeAmount> OfficialFees { get;set; }
        public IList<FeeAmount> PartnerFees { get; set; }
        public IList<FeeAmount> ServiceFees { get; set; }
        public IList<FeeResultFail> Errors { get; set; }
    }
}
