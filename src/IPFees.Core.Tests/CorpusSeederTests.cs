using IPFees.Core.FeeCalculation;
using IPFees.Core.Tests.Fixture;
using IPFLang.Corpus;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// End to end over the real path a deployment takes: the jurisdiction corpus travels inside
    /// the IPFLang package, gets loaded into the database, and is then executed from there.
    /// A jurisdiction that extends a regional base has to survive the round trip with its
    /// inheritance intact, which is the part most likely to break silently.
    /// </summary>
    public class CorpusSeederTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public CorpusSeederTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        private CorpusSeeder NewSeeder() =>
            new(fixture.JurisdictionRepository, fixture.FeeRepository, fixture.ModuleRepository);

        private FeeCalculator NewCalculator() =>
            new(fixture.FeeRepository,
                fixture.ModuleRepository,
                new DslCalculator(new DslParser()),
                new FeeScriptComposer(new DslParser()));

        [Fact]
        public async Task SeedsEveryJurisdictionAndBaseFromThePackage()
        {
            var report = await NewSeeder().SeedAsync();

            Assert.True(report.Succeeded, string.Join(" | ", report.Errors));
            Assert.Equal(JurisdictionCorpus.Codes.Count, report.Jurisdictions);
            Assert.Equal(JurisdictionCorpus.BaseNames.Count, report.Bases);

            var stored = await fixture.FeeRepository.GetFees();
            foreach (var code in JurisdictionCorpus.Codes)
            {
                Assert.Contains(stored, f => f.Name.Equals($"PCT-{code}", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public async Task SeedingTwiceLeavesOneCopyOfEachJurisdiction()
        {
            await NewSeeder().SeedAsync();
            await NewSeeder().SeedAsync();

            var stored = (await fixture.FeeRepository.GetFees())
                .Where(f => f.Name.StartsWith("PCT-", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var duplicated = stored.GroupBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.True(duplicated.Count == 0, $"seeding was not idempotent: {string.Join(", ", duplicated)}");
        }

        [Fact]
        public async Task AJurisdictionExtendingARegionalBaseComputesFromTheDatabase()
        {
            await NewSeeder().SeedAsync();

            // Romania enters the national phase through the European Patent Convention, so its
            // schedule inherits base_ep. If the base did not survive seeding, the inherited
            // fees would simply be absent and the total would be wrong rather than failing.
            var romania = (await fixture.FeeRepository.GetFees())
                .Single(f => f.Name.Equals("PCT-RO", StringComparison.OrdinalIgnoreCase));

            Assert.NotEmpty(romania.ReferencedModules);

            var calculator = NewCalculator();

            var parsed = calculator.GetInputs(romania.Id);
            var inputs = Assert.IsType<FeeResultParse>(parsed);
            Assert.NotEmpty(inputs.FeeInputs);

            var result = calculator.Calculate(romania.Id, DefaultsFor(inputs.FeeInputs));
            var computed = Assert.IsType<FeeResultCalculation>(result);

            Assert.True(computed.TotalMandatoryAmount > 0, "Romania should charge a mandatory national fee");
            Assert.Contains(computed.Returns, r => r.Item1.Equals("Currency", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task AStandaloneJurisdictionComputesFromTheDatabase()
        {
            await NewSeeder().SeedAsync();

            // The United States belongs to no regional system, so it carries no base.
            var usa = (await fixture.FeeRepository.GetFees())
                .Single(f => f.Name.Equals("PCT-US", StringComparison.OrdinalIgnoreCase));

            Assert.Empty(usa.ReferencedModules);

            var calculator = NewCalculator();
            var inputs = Assert.IsType<FeeResultParse>(calculator.GetInputs(usa.Id));
            var computed = Assert.IsType<FeeResultCalculation>(calculator.Calculate(usa.Id, DefaultsFor(inputs.FeeInputs)));

            Assert.True(computed.TotalMandatoryAmount > 0);
        }

        /// <summary>
        /// Schedules whose directives are all VERIFY COMPLETE settle in milliseconds and must
        /// hold. Schedules that also declare VERIFY MONOTONIC are deliberately excluded here;
        /// see <see cref="MonotonicityCheckingDoesNotScaleToWideSchedules"/> for why.
        /// </summary>
        [Theory]
        [InlineData("PCT-RO")]
        [InlineData("PCT-SG")]
        public async Task CompletenessOnlySchedulesPassVerificationQuickly(string name)
        {
            await NewSeeder().SeedAsync();

            var fee = (await fixture.FeeRepository.GetFees())
                .Single(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            var started = DateTime.UtcNow;
            var result = NewCalculator().Verify(fee.Id);
            var elapsed = DateTime.UtcNow - started;

            var verification = Assert.IsType<FeeResultVerification>(result);
            Assert.True(verification.Results.AllPassed, verification.Results.ToString());
            Assert.True(elapsed < TimeSpan.FromSeconds(10), $"{name} took {elapsed.TotalSeconds:N1}s to verify");
        }

        /// <summary>
        /// A standing record of a real limit in the engine, not an aspiration.
        ///
        /// Completeness checking finishes in single-digit milliseconds on these schedules.
        /// Monotonicity checking walks a numeric domain crossed with every combination of the
        /// remaining categorical inputs, and on a schedule as wide as the corpus jurisdictions
        /// it does not finish in any usable time: measured at over 60 seconds for PCT-US,
        /// PCT-DE, PCT-EP and PCT-MY, against 1-5 ms for the completeness-only schedules.
        /// 46 of the 119 corpus schedules declare a MONOTONIC directive.
        ///
        /// The editor therefore bounds verification with <see cref="VerificationBudget"/> and
        /// reports an unfinished run rather than blocking the author. This test pins that
        /// behaviour: the budget must be honoured, and the result must say plainly that nothing
        /// was proven.
        /// </summary>
        [Fact]
        public async Task MonotonicityCheckingDoesNotScaleToWideSchedules()
        {
            var budget = new VerificationBudget { Limit = TimeSpan.FromSeconds(3) };
            var validator = new FeeDefinitionValidator(
                fixture.ModuleRepository,
                new FeeScriptComposer(new DslParser()),
                new DslCalculator(new DslParser()),
                budget);

            var source = JurisdictionCorpus.GetJurisdiction("US");

            var started = DateTime.UtcNow;
            var report = await validator.ValidateAsync(source, Array.Empty<Guid>());
            var elapsed = DateTime.UtcNow - started;

            Assert.True(report.CanBeStored, "the schedule itself is well formed");
            Assert.True(report.VerificationTimedOut, "expected verification to overrun the budget");
            Assert.True(elapsed < TimeSpan.FromSeconds(20), $"budget was not honoured: waited {elapsed.TotalSeconds:N1}s");

            // Nothing was proven, so nothing may be claimed either way.
            Assert.Empty(report.CompletenessFailures);
            Assert.Empty(report.MonotonicityFailures);
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
