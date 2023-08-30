using IPFees.Core.Data;
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
        private readonly ICurrencyConverter currencyConverter;

        public JurisdictionFeeManager(IFeeCalculator feeCalculator, IFeeRepository feeRepository, IJurisdictionRepository jurisdictionRepository, ICurrencyConverter currencyConverter, ISettingsRepository settingsRepository)
        {
            this.feeCalculator = feeCalculator;
            this.feeRepository = feeRepository;
            this.jurisdictionRepository = jurisdictionRepository;
            this.settingsRepository = settingsRepository;
            this.currencyConverter = currencyConverter;
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
        /// <param name="TargetCurrency">Currency to which all amounts will eventually be converted to</param>
        /// <returns>A struct containing the fees for all jurisdictions as well as totals</returns>
        public async Task<TotalFeeInfo> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues, string TargetCurrency)
        {
            /// There are four types of fees that are calculated for each jurisdiction:
            /// - Official fee - the amount paid to the government receiving the application
            /// - Partner fee - the amount paid to a partner in the respective jurisdiction, which does the translation and the actual application fileing work
            /// - Service fee - the amount paid to the partner which orchestrates the filing of the aplication, i.e. collaborates with the client for each individual jurisdiction
            /// - Translation fee - the amount paid for translation of the documents
            /// Note: Each individual fee has its associated currency. At a later stage, the user shall convert the source currencies into whatever final target currency may be (usually EUR or USD).
            var Errors = new List<FeeResultFail>();
            var JurisdictionFees = new List<JurisdictionFeesAmount>();

            // Cycle through each required jurisdiction            
            foreach (var jn in JurisdictionNames)
            {
                var CurrentJurisdictionFees = new JurisdictionFeesAmount(jn, new FeeAmount(0, 0, string.Empty), new FeeAmount(0, 0, string.Empty), new FeeAmount(0, 0, string.Empty), new FeeAmount(0, 0, string.Empty), new FeeAmount(0, 0, string.Empty));
                // Calculate the associated service fee (the simplest of all fees)
                // Each jurisdiction has an associated service fee level (e.g. Level 1, 2, a.s.o.) and each level has an associated monetary value and currency
                #region Service Fee
                var jur = await jurisdictionRepository.GetJurisdictionByName(jn);
                var ServiceFee = await settingsRepository.GetServiceFeeAsync(jur.ServiceFeeLevel);
                CurrentJurisdictionFees = CurrentJurisdictionFees with { ServiceFee = new FeeAmount(ServiceFee.Amount, 0, ServiceFee.Currency) };
                #endregion

                // Calculate the official and partner fees
                #region Official, Translation, and Partner Fees
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
                                var f1 = new FeeAmount(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty)
                                    );
                                CurrentJurisdictionFees = CurrentJurisdictionFees with { OfficialFee = f1 };
                                break;
                            case FeeCategory.PartnerFees:
                                var f2 = new FeeAmount(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty)
                                    );
                                CurrentJurisdictionFees = CurrentJurisdictionFees with { PartnerFee = f2 };
                                break;
                            case FeeCategory.TranslationFees:
                                var f3 = new FeeAmount(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty)
                                    );
                                CurrentJurisdictionFees = CurrentJurisdictionFees with { TranslationFee = f3 };
                                break;
                        }
                    }
                }
                #endregion
                // Compute total for the current jurisdiction
                var TotalFee = FeeAmount.Add(CurrentJurisdictionFees.OfficialFee, CurrentJurisdictionFees.PartnerFee);
                TotalFee = FeeAmount.Add(TotalFee, CurrentJurisdictionFees.TranslationFee);
                TotalFee = FeeAmount.Add(TotalFee, CurrentJurisdictionFees.ServiceFee);
                CurrentJurisdictionFees = CurrentJurisdictionFees with { TotalFee = TotalFee };
                // TODO: Convert fees to target currency

                // Store fees for the current jurisdiction
                JurisdictionFees.Add(CurrentJurisdictionFees);
            }
            // Compute totals
            var TotalOfficialFee = new FeeAmount(JurisdictionFees.Sum(s1 => s1.OfficialFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.OfficialFee.OptionalAmount), TargetCurrency);
            var TotalPartnerFee = new FeeAmount(JurisdictionFees.Sum(s1 => s1.PartnerFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.PartnerFee.OptionalAmount), TargetCurrency);
            var TotalTranslationFee = new FeeAmount(JurisdictionFees.Sum(s1 => s1.TranslationFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.TranslationFee.OptionalAmount), TargetCurrency);
            var TotalServiceFee = new FeeAmount(JurisdictionFees.Sum(s1 => s1.ServiceFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.ServiceFee.OptionalAmount), TargetCurrency);

            var GrandTotalFee = FeeAmount.Add(TotalOfficialFee, TotalPartnerFee);
            GrandTotalFee = FeeAmount.Add(GrandTotalFee, TotalTranslationFee);
            GrandTotalFee = FeeAmount.Add(GrandTotalFee, TotalServiceFee);

            return new TotalFeeInfo
            {
                JurisdictionFees = JurisdictionFees,
                TotalOfficialFee = TotalOfficialFee,
                TotalPartnerFee = TotalPartnerFee,
                TotalTranslationFee = TotalTranslationFee,
                TotalServiceFee = TotalServiceFee,
                GrandTotalFee = GrandTotalFee,
                Errors = Errors
            };
        }

        private IEnumerable<FeeInfo> GetFeeDefinitionForJurisdiction(string JurisdictionName) => feeRepository.GetFees().Result.Where(w => w.JurisdictionName.Equals(JurisdictionName));
    }

    public record JurisdictionFeesAmount(string Jurisdiction, FeeAmount OfficialFee, FeeAmount PartnerFee, FeeAmount TranslationFee, FeeAmount ServiceFee, FeeAmount TotalFee);


    public class TotalFeeInfo
    {
        public List<JurisdictionFeesAmount> JurisdictionFees { get; set; }
        public FeeAmount TotalOfficialFee { get; set; }
        public FeeAmount TotalPartnerFee { get; set; }
        public FeeAmount TotalTranslationFee { get; set; }
        public FeeAmount TotalServiceFee { get; set; }
        public FeeAmount GrandTotalFee { get; set; }
        public IList<FeeResultFail> Errors { get; set; }
    }
}
