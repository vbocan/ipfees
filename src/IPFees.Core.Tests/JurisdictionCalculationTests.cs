using IPFees.Core.CurrencyConversion;
using IPFees.Core.Enum;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Tests.Fixture;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// Exercises the whole calculation path the web interface and the REST API both use:
    /// seeded corpus, composed schedule, currency conversion, service fee, totals.
    ///
    /// The unit tests around FeeCalculator stop at a single fee definition and never touch
    /// currency, which is how a defect in the accumulation reached production: Fee.Add refuses
    /// to combine two currencies, and an accumulator seeded with no currency at all threw on
    /// its first addition, failing every calculation.
    /// </summary>
    public class JurisdictionCalculationTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public JurisdictionCalculationTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        /// <summary>
        /// Converts nothing. Every schedule under test is quoted in the target currency, so a
        /// real rate provider would add a network dependency without adding coverage.
        /// </summary>
        private sealed class IdentityConverter : ICurrencyConverter
        {
            public ExchangeRateResponse Response { get; set; } =
                new(ResponseStatus.ResponseOnline, "test", new Dictionary<string, decimal>(), DateTime.UtcNow);
            public decimal ConvertCurrency(decimal amount, string source, string target) => amount;
            public IEnumerable<(string, string)> GetCurrencies() => Array.Empty<(string, string)>();
        }

        private async Task<JurisdictionFeeManager> SeedAndBuildManager()
        {
            var seeder = new CorpusSeeder(fixture.JurisdictionRepository, fixture.FeeRepository, fixture.ModuleRepository);
            var report = await seeder.SeedAsync();
            Assert.True(report.Succeeded, string.Join(" | ", report.Errors));

            Assert.True((await fixture.SettingsRepository.SetServiceFeeAsync(ServiceFeeLevel.Level1, 250M, "EUR")).Success);

            var calculator = new FeeCalculator(
                fixture.FeeRepository,
                fixture.ModuleRepository,
                new DslCalculator(new DslParser()),
                new FeeScriptComposer(new DslParser()));

            return new JurisdictionFeeManager(
                calculator,
                fixture.FeeRepository,
                fixture.JurisdictionRepository,
                fixture.SettingsRepository,
                new IdentityConverter());
        }

        [Fact]
        public async Task CalculatingASingleJurisdictionReportsNoErrors()
        {
            var manager = await SeedAndBuildManager();

            var (inputs, _, inputErrors) = manager.GetConsolidatedInputs(new[] { "RO" });
            Assert.Empty(inputErrors);

            var result = await manager.Calculate(new[] { "RO" }, DefaultsFor(inputs), "EUR", 0M);

            Assert.True(result.Errors.Count == 0, string.Join(" | ", result.Errors.SelectMany(e => e.Errors)));
            var romania = Assert.Single(result.JurisdictionFees);

            Assert.Equal("RO", romania.Jurisdiction);
            Assert.True(romania.Fees.MandatoryAmount > 0, "Romania charges a mandatory national fee");
            Assert.Equal(250M, romania.ServiceFee.MandatoryAmount);
            Assert.Equal("EUR", romania.TotalFee.Currency);

            // The jurisdiction total is its own fees plus the service fee, nothing else.
            Assert.Equal(romania.Fees.MandatoryAmount + romania.ServiceFee.MandatoryAmount, romania.TotalFee.MandatoryAmount);
        }

        [Fact]
        public async Task TotalsAcrossJurisdictionsAreTheSumOfTheParts()
        {
            var manager = await SeedAndBuildManager();
            var wanted = new[] { "RO", "SG" };

            var (inputs, _, _) = manager.GetConsolidatedInputs(wanted);
            var result = await manager.Calculate(wanted, DefaultsFor(inputs), "EUR", 0M);

            Assert.True(result.Errors.Count == 0, string.Join(" | ", result.Errors.SelectMany(e => e.Errors)));
            Assert.Equal(2, result.JurisdictionFees.Count);

            Assert.Equal(result.JurisdictionFees.Sum(j => j.Fees.MandatoryAmount), result.TotalFees.MandatoryAmount);
            Assert.Equal(result.JurisdictionFees.Sum(j => j.ServiceFee.MandatoryAmount), result.TotalServiceFee.MandatoryAmount);
            Assert.Equal(result.TotalFees.MandatoryAmount + result.TotalServiceFee.MandatoryAmount, result.GrandTotalFee.MandatoryAmount);
        }

        [Fact]
        public async Task EveryJurisdictionInTheCorpusCalculatesWithoutError()
        {
            // Catches a schedule that composes and type-checks but cannot produce a total,
            // for instance one that never declares its currency.
            var manager = await SeedAndBuildManager();
            var failures = new List<string>();

            foreach (var code in IPFLang.Corpus.JurisdictionCorpus.Codes)
            {
                var (inputs, _, inputErrors) = manager.GetConsolidatedInputs(new[] { code });
                if (inputErrors.Any())
                {
                    failures.Add($"{code}: {string.Join(" | ", inputErrors.SelectMany(e => e.Errors))}");
                    continue;
                }

                var result = await manager.Calculate(new[] { code }, DefaultsFor(inputs), "EUR", 0M);
                if (result.Errors.Count > 0)
                {
                    failures.Add($"{code}: {string.Join(" | ", result.Errors.SelectMany(e => e.Errors))}");
                }
                else if (result.JurisdictionFees.Count != 1)
                {
                    failures.Add($"{code}: expected one result, got {result.JurisdictionFees.Count}");
                }
            }

            Assert.True(failures.Count == 0, $"{failures.Count} jurisdictions failed:{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(15))}");
        }

        private static List<IPFValue> DefaultsFor(IEnumerable<DslInput> inputs)
        {
            var values = new List<IPFValue>();
            foreach (var input in inputs)
            {
                switch (input)
                {
                    case DslInputNumber n: values.Add(new IPFValueNumber(input.Name, n.DefaultValue)); break;
                    case DslInputBoolean b: values.Add(new IPFValueBoolean(input.Name, b.DefaultValue)); break;
                    case DslInputList l: values.Add(new IPFValueString(input.Name, l.DefaultSymbol)); break;
                    case DslInputListMultiple m: values.Add(new IPFValueStringList(input.Name, m.DefaultSymbols.ToList())); break;
                    case DslInputDate d: values.Add(new IPFValueDate(input.Name, d.DefaultValue)); break;
                    case DslInputAmount a: values.Add(new IPFValueNumber(input.Name, a.DefaultValue)); break;
                }
            }
            return values;
        }
    }
}
