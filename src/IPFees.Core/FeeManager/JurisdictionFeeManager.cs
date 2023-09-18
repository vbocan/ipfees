using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.FeeCalculation;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFees.Evaluator;
using IPFees.Parser;
using MongoDB.Driver;

namespace IPFees.Core.FeeManager
{
    public class JurisdictionFeeManager : IJurisdictionFeeManager
    {
        private readonly IFeeCalculator feeCalculator;
        private readonly IFeeRepository feeRepository;
        private readonly ISettingsRepository settingsRepository;
        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly ICurrencyConverter currencyConverter;

        public JurisdictionFeeManager(IFeeCalculator feeCalculator, IFeeRepository feeRepository, IJurisdictionRepository jurisdictionRepository, ISettingsRepository settingsRepository, ICurrencyConverter currencyConverter)
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
        /// <param name="CurrencyMarkup">Percent to be added to monetary conversions to offset the exchange rate risk</param>
        /// <returns>A struct containing the fees for all jurisdictions as well as totals</returns>
        public async Task<TotalFeeInfo> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues, string TargetCurrency, decimal CurrencyMarkup)
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
                // Calculate the associated service fee (the simplest of all fees)
                // Each jurisdiction has an associated service fee level (e.g. Level 1, 2, a.s.o.) and each level has an associated monetary value and currency
                #region Service Fee
                var jur = await jurisdictionRepository.GetJurisdictionByName(jn);
                var DbFee1 = await settingsRepository.GetServiceFeeAsync(jur.ServiceFeeLevel);
                var ServiceFee = new Fee(DbFee1.Amount, 0, DbFee1.Currency);
                #endregion

                // Calculate the official and partner fees
                #region Official, Translation, and Partner Fees
                // For the current jurisdiction, we need the associated fees defined in the system. These fees have different purposes (e.g. OfficialFee and PartnerFee) and we
                // need to discriminate between those.
                var FeeDefinitions = GetFeeDefinitionForJurisdiction(jn);
                // Let's tackle one fee definition at a time
                var OfficialFee = new Fee(0.0M, 0.0M, string.Empty);
                var PartnerFee = new Fee(0.0M, 0.0M, string.Empty);
                var TranslationFee = new Fee(0.0M, 0.0M, string.Empty);
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
                                OfficialFee = new Fee(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty)
                                    );
                                break;
                            case FeeCategory.PartnerFees:
                                PartnerFee = new Fee(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty)
                                    );
                                break;
                            case FeeCategory.TranslationFees:
                                TranslationFee = new Fee(
                                    frc.TotalMandatoryAmount,
                                    frc.TotalOptionalAmount,
                                    frc.Returns.Where(w => w.Item1.Equals("Currency", StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Item2 ?? string.Empty).SingleOrDefault(string.Empty)
                                    );
                                break;
                        }
                    }
                }
                #endregion
                // Check whether the current jurisdiction has all fees defined, otherwise signal error
                if(string.IsNullOrEmpty(OfficialFee.Currency))
                {
                    Errors.Add(new FeeResultFail(jur.Name, jur.Description, new string[] { "[Official Fee] definition is missing" }));
                    continue;
                }
                if (string.IsNullOrEmpty(PartnerFee.Currency))
                {
                    Errors.Add(new FeeResultFail(jur.Name, jur.Description, new string[] { "[Partner Fee] definition is missing" }));
                    continue;
                }
                if (string.IsNullOrEmpty(TranslationFee.Currency))
                {
                    Errors.Add(new FeeResultFail(jur.Name, jur.Description, new string[] { "[Translation Fee] definition is missing" }));
                    continue;
                }
                // Convert the Official Fee
                var ConvertedOfficialFee = ConvertCurrency(OfficialFee, TargetCurrency, CurrencyMarkup);
                // Convert the Partner Fee
                var ConvertedPartnerFee = ConvertCurrency(PartnerFee, TargetCurrency, CurrencyMarkup);
                // Convert the Translation Fee
                var ConvertedTranslationFee = ConvertCurrency(TranslationFee, TargetCurrency, CurrencyMarkup);
                // Convert the Service Fee
                var ConvertedServiceFee = ConvertCurrency(ServiceFee, TargetCurrency, CurrencyMarkup);

                // Compute total for the current jurisdiction
                var ConvertedTotalFee = Fee.Add(ConvertedOfficialFee, ConvertedPartnerFee);
                ConvertedTotalFee = Fee.Add(ConvertedTotalFee, ConvertedTranslationFee);
                ConvertedTotalFee = Fee.Add(ConvertedTotalFee, ConvertedServiceFee);

                var CurrentJurisdictionFees = new JurisdictionFeesAmount(jn, "N/A", ConvertedOfficialFee, ConvertedPartnerFee, ConvertedTranslationFee, ConvertedServiceFee, ConvertedTotalFee);
                // Store fees for the current jurisdiction
                JurisdictionFees.Add(CurrentJurisdictionFees);
            }
            // Compute totals
            var TotalOfficialFee = new Fee(JurisdictionFees.Sum(s1 => s1.OfficialFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.OfficialFee.OptionalAmount), TargetCurrency);
            var TotalPartnerFee = new Fee(JurisdictionFees.Sum(s1 => s1.PartnerFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.PartnerFee.OptionalAmount), TargetCurrency);
            var TotalTranslationFee = new Fee(JurisdictionFees.Sum(s1 => s1.TranslationFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.TranslationFee.OptionalAmount), TargetCurrency);
            var TotalServiceFee = new Fee(JurisdictionFees.Sum(s1 => s1.ServiceFee.MandatoryAmount), JurisdictionFees.Sum(s2 => s2.ServiceFee.OptionalAmount), TargetCurrency);

            var GrandTotalFee = Fee.Add(TotalOfficialFee, TotalPartnerFee);
            GrandTotalFee = Fee.Add(GrandTotalFee, TotalTranslationFee);
            GrandTotalFee = Fee.Add(GrandTotalFee, TotalServiceFee);

            return new TotalFeeInfo(JurisdictionFees, TotalOfficialFee, TotalPartnerFee, TotalTranslationFee, TotalServiceFee, GrandTotalFee, Errors);
        }

        private IEnumerable<FeeInfo> GetFeeDefinitionForJurisdiction(string JurisdictionName) => feeRepository.GetFees().Result.Where(w => w.JurisdictionName.Equals(JurisdictionName));

        /// <summary>
        /// Convert a Fee from the original currency to a specified target currency
        /// </summary>
        /// <param name="SourceFee">Fee to convert</param>
        /// <param name="TargetCurrency">Target currency for the converted fee</param>
        /// <param name="CurrencyMarkup">Percent to be added to monetary conversions to offset the exchange rate risk</param>
        /// <returns>The converted fee</returns>
        private Fee ConvertCurrency(Fee SourceFee, string TargetCurrency, decimal CurrencyMarkup)
        {
            // Compute the monetary value in the TargetCurrency
            var ma = currencyConverter.ConvertCurrency(SourceFee.MandatoryAmount, SourceFee.Currency, TargetCurrency);
            var oa = currencyConverter.ConvertCurrency(SourceFee.OptionalAmount, SourceFee.Currency, TargetCurrency);
            // Add the specified currency markup
            var mam = ma + (ma * CurrencyMarkup / 100M);
            var oam = oa + (oa * CurrencyMarkup / 100M);
            return new Fee(Math.Round(mam), Math.Round(oam), TargetCurrency);
        }
    }

    public record JurisdictionFeesAmount(string Jurisdiction, string Language, Fee OfficialFee, Fee PartnerFee, Fee TranslationFee, Fee ServiceFee, Fee TotalFee);

    public record TotalFeeInfo(List<JurisdictionFeesAmount> JurisdictionFees, Fee TotalOfficialFee, Fee TotalPartnerFee, Fee TotalTranslationFee, Fee TotalServiceFee, Fee GrandTotalFee, IList<FeeResultFail> Errors);
}
