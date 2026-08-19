using IPFees.Core.CurrencyConversion;
using IPFees.Core.Data;
using IPFees.Core.Enum;
using IPFees.Core.FeeCalculation;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFLang.Evaluator;
using IPFLang.Parser;
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
                        Errors.Add((FeeResultFail)inp);
                    }
                    else
                    {
                        var fps = inp as FeeResultParse;
                        Inputs.AddRange(fps!.FeeInputs);
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
            // A jurisdiction is one IPFLang script. Executing it yields a mandatory total and
            // an optional total in the script's own currency; optional covers the components a
            // filing may or may not incur, such as examination, translation or agent work.
            //
            // The service fee is the one amount that does not come from the script. It is what
            // the party orchestrating the filing charges, held per jurisdiction as a level in
            // settings, so it is calculated and reported separately.
            var Errors = new List<FeeResultFail>();
            var JurisdictionFees = new List<JurisdictionFeesAmount>();

            foreach (var jn in JurisdictionNames)
            {
                var jur = await jurisdictionRepository.GetJurisdictionByName(jn);
                var configured = await settingsRepository.GetServiceFeeAsync(jur.ServiceFeeLevel);
                var ServiceFee = new Fee(configured.Amount, 0, configured.Currency);

                var Fees = new Fee(0M, 0M, string.Empty);
                var Language = "N/A";
                var Calculated = false;
                var Failed = false;

                foreach (var fd in GetFeeDefinitionForJurisdiction(jn))
                {
                    var res = feeCalculator.Calculate(fd.Id, InputValues);
                    if (res is FeeResultFail fail)
                    {
                        Errors.Add(fail);
                        Failed = true;
                        continue;
                    }

                    var frc = (FeeResultCalculation)res;
                    var Currency = Returned(frc, "Currency");
                    if (string.IsNullOrEmpty(Currency))
                    {
                        Errors.Add(new FeeResultFail(fd.Name, fd.Description, new[] { $"'{fd.Name}' does not declare a currency. Add RETURN Currency AS '<ISO 4217 code>'." }));
                        Failed = true;
                        continue;
                    }

                    var Declared = Returned(frc, "Language");
                    if (!string.IsNullOrEmpty(Declared)) Language = Declared;

                    // Convert each definition from its own currency before accumulating, so a
                    // jurisdiction whose components are quoted in different currencies still adds up.
                    Fees = Fee.Add(Fees, ConvertCurrency(new Fee(frc.TotalMandatoryAmount, frc.TotalOptionalAmount, Currency), TargetCurrency, CurrencyMarkup));
                    Calculated = true;
                }

                if (Failed) continue;

                if (!Calculated)
                {
                    Errors.Add(new FeeResultFail(jur.Name, jur.Description, new[] { "No fee definition is registered for this jurisdiction." }));
                    continue;
                }

                try
                {
                    var ConvertedServiceFee = ConvertCurrency(ServiceFee, TargetCurrency, CurrencyMarkup);
                    var Total = Fee.Add(Fees, ConvertedServiceFee);
                    JurisdictionFees.Add(new JurisdictionFeesAmount(jn, Language, Fees, ConvertedServiceFee, Total));
                }
                catch (Exception ex)
                {
                    Errors.Add(new FeeResultFail(jur.Name, jur.Description, new[] { ex.Message }));
                }
            }

            var TotalFees = new Fee(JurisdictionFees.Sum(s => s.Fees.MandatoryAmount), JurisdictionFees.Sum(s => s.Fees.OptionalAmount), TargetCurrency);
            var TotalServiceFee = new Fee(JurisdictionFees.Sum(s => s.ServiceFee.MandatoryAmount), JurisdictionFees.Sum(s => s.ServiceFee.OptionalAmount), TargetCurrency);
            var GrandTotalFee = Fee.Add(TotalFees, TotalServiceFee);

            return new TotalFeeInfo(JurisdictionFees, TotalFees, TotalServiceFee, GrandTotalFee, Errors);
        }

        /// <summary>Read a named RETURN value from a completed calculation.</summary>
        private static string Returned(FeeResultCalculation result, string name) =>
            result.Returns
                .Where(w => w.Item1.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Select(s => s.Item2 ?? string.Empty)
                .FirstOrDefault(string.Empty);

        public IEnumerable<FeeVerificationInfo> Verify(IEnumerable<string> JurisdictionNames)
        {
            foreach (var jn in JurisdictionNames)
            {
                foreach (var fd in GetFeeDefinitionForJurisdiction(jn))
                {
                    var result = feeCalculator.Verify(fd.Id);

                    if (result is FeeResultFail fail)
                    {
                        yield return new FeeVerificationInfo(jn, fd.Name, fd.Category.ToString(), 0, false,
                            Array.Empty<string>(), Array.Empty<string>(), fail.Errors.ToList());
                        continue;
                    }

                    var results = ((FeeResultVerification)result).Results;

                    var completeness = results.CompletenessReports
                        .Where(r => !r.IsComplete)
                        .Select(VerificationNarrative.Describe)
                        .ToList();

                    var monotonicity = results.MonotonicityReports
                        .Where(r => !r.IsMonotonic)
                        .Select(VerificationNarrative.Describe)
                        .ToList();

                    var declared = results.CompletenessReports.Count + results.MonotonicityReports.Count;

                    yield return new FeeVerificationInfo(
                        jn, fd.Name, fd.Category.ToString(), declared,
                        results.AllPassed, completeness, monotonicity, results.Errors.ToList());
                }
            }
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
            // Avoid converting the same currency (markup would be applied needlessly)
            if (SourceFee.Currency.Equals(TargetCurrency)) return SourceFee;
            // Compute the monetary value in the TargetCurrency
            var ma = currencyConverter.ConvertCurrency(SourceFee.MandatoryAmount, SourceFee.Currency, TargetCurrency);
            var oa = currencyConverter.ConvertCurrency(SourceFee.OptionalAmount, SourceFee.Currency, TargetCurrency);
            // Add the specified currency markup
            var mam = ma + (ma * CurrencyMarkup / 100M);
            var oam = oa + (oa * CurrencyMarkup / 100M);
            return new Fee(Math.Round(mam), Math.Round(oam), TargetCurrency);
        }
    }

    /// <param name="Fees">Everything the jurisdiction's own schedule produced, mandatory and optional.</param>
    /// <param name="ServiceFee">Filing-orchestration charge, held in settings rather than in the schedule.</param>
    public record JurisdictionFeesAmount(string Jurisdiction, string Language, Fee Fees, Fee ServiceFee, Fee TotalFee);

    public record TotalFeeInfo(List<JurisdictionFeesAmount> JurisdictionFees, Fee TotalFees, Fee TotalServiceFee, Fee GrandTotalFee, IList<FeeResultFail> Errors);
}
