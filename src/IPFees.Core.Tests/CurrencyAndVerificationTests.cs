using System.Text;
using IPFees.Core.Enum;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Core.Tests.Fixture;
using IPFLang.Composition;
using IPFLang.Corpus;
using IPFLang.CurrencyConversion;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;
using IPFLang.Versioning;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// A fee is converted from the currency it is priced in, not from the schedule's, and the
    /// save gate reports every directive that finished even when another one did not.
    /// </summary>
    public class CurrencyAndVerificationTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public CurrencyAndVerificationTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        private static ParsedScript Parse(string source)
        {
            var (script, errors) = new DslParser().Parse(source, returnParsedScript: true);
            Assert.True(script is not null, string.Join(" | ", errors));
            return script!;
        }

        private static DslFee Fee(ParsedScript script, string name) => script.Fees.Single(f => f.Name == name);

        private const string MixedSchedule = """
            DEFINE NUMBER ClaimCount AS 'Number of claims'
            BETWEEN 1 AND 100
            DEFAULT 10
            ENDDEFINE

            COMPUTE FEE OfficialFee
            YIELD 200<SGD>
            YIELD 40 * (ClaimCount - 5) IF ClaimCount GT 5
            ENDCOMPUTE

            COMPUTE FEE PlainFee OPTIONAL
            YIELD 12 * ClaimCount
            ENDCOMPUTE

            COMPUTE FEE AgentServiceFee OPTIONAL
            YIELD 100<EUR>
            ENDCOMPUTE

            RETURN Currency AS 'SGD'
            """;

        [Fact]
        public void AFeeIsInTheCurrencyOfItsLiterals()
        {
            var script = Parse(MixedSchedule);

            Assert.Equal("SGD", FeeCurrency.Resolve(Fee(script, "OfficialFee"), script.Inputs, "SGD"));
            Assert.Equal("EUR", FeeCurrency.Resolve(Fee(script, "AgentServiceFee"), script.Inputs, "SGD"));
        }

        [Fact]
        public void AFeeOfPlainNumbersIsInTheScheduleCurrency()
        {
            var script = Parse(MixedSchedule);
            Assert.Equal("SGD", FeeCurrency.Resolve(Fee(script, "PlainFee"), script.Inputs, "SGD"));
        }

        /// <summary>
        /// Every schedule in the reference corpus resolves each of its fees to one currency, and
        /// the agent's service charge, written in EUR, is reported as EUR wherever the schedule
        /// itself prices in something else.
        /// </summary>
        [Fact]
        public void EveryCorpusFeeResolvesToOneCurrency()
        {
            var agentInEur = 0;
            foreach (var code in JurisdictionCorpus.Codes)
            {
                var script = Compose(JurisdictionCorpus.GetCompositionChain(code));
                var currency = script.Returns.First(r => r.Symbol == "Currency").Text;

                foreach (var fee in script.Fees)
                {
                    var resolved = FeeCurrency.Resolve(fee, script.Inputs, currency);
                    if (fee.Name == "AgentServiceFee" && resolved == "EUR") agentInEur++;
                }
            }

            Assert.Equal(JurisdictionCorpus.Codes.Count(), agentInEur);
        }

        /// <summary>Converts SGD to EUR at a fixed half, and leaves everything else alone.</summary>
        private sealed class HalvingSgdConverter : ICurrencyConverter
        {
            public ExchangeRateResponse Response { get; set; } =
                new(ResponseStatus.Online, "test", new Dictionary<string, decimal>(), DateTime.UtcNow);
            public decimal Convert(decimal amount, string source, string target) =>
                source == "SGD" && target == "EUR" ? amount / 2 : source == target ? amount : throw new NotSupportedException($"{source}->{target}");
            public decimal GetRate(string source, string target) => source == "SGD" ? 0.5m : 1m;
            public IEnumerable<(string Code, string Name)> GetCurrencies() => Array.Empty<(string, string)>();
        }

        [Fact]
        public async Task EachFeeIsConvertedFromItsOwnCurrency()
        {
            var add = await fixture.FeeRepository.AddFeeAsync("MIXED-TEST");
            Assert.True(add.Success, add.Reason);
            await fixture.FeeRepository.SetFeeSourceCodeAsync(add.Id, MixedSchedule);
            await fixture.FeeRepository.SetFeeJurisdictionNameAsync(add.Id, "ZZ");
            await fixture.FeeRepository.SetFeeCategoryAsync(add.Id, FeeCategory.OfficialFees);
            var jur = await fixture.JurisdictionRepository.AddJurisdictionAsync("ZZ");
            if (jur.Success) await fixture.JurisdictionRepository.SetJurisdictionServiceFeeLevelAsync(jur.Id, ServiceFeeLevel.Level1);
            await fixture.SettingsRepository.SetServiceFeeAsync(ServiceFeeLevel.Level1, 0M, "EUR");

            var calculator = new FeeCalculator(fixture.FeeRepository, fixture.ModuleRepository,
                new DslCalculator(new DslParser()), new FeeScriptComposer(new DslParser()));
            var manager = new JurisdictionFeeManager(calculator, fixture.FeeRepository,
                fixture.JurisdictionRepository, fixture.SettingsRepository, new HalvingSgdConverter());

            var inputs = new List<IPFValue> { new IPFValueNumber("ClaimCount", 10) };
            var computed = Assert.IsType<FeeResultCalculation>(calculator.Calculate(add.Id, inputs));
            Assert.NotNull(computed.Components);
            Assert.Equal("EUR", computed.Components!.Single(c => c.Name == "AgentServiceFee").Currency);

            var result = await manager.Calculate(new[] { "ZZ" }, inputs, "EUR", 0M);
            Assert.True(result.Errors.Count == 0, string.Join(" | ", result.Errors.SelectMany(e => e.Errors)));
            var zz = Assert.Single(result.JurisdictionFees);

            // Mandatory: (200 + 40 * 5) SGD = 400 SGD = 200 EUR.
            // Optional: 120 SGD = 60 EUR, plus the agent's 100 EUR taken as it stands.
            Assert.Equal(200M, zz.Fees.MandatoryAmount);
            Assert.Equal(160M, zz.Fees.OptionalAmount);
        }

        /// <summary>
        /// Handing the monotonicity checker only the inputs a fee reads must not hide a violation.
        /// The schedule below declares inputs the fee never reads; the fee falls as the claim
        /// count rises, against its NonDecreasing directive.
        /// </summary>
        [Fact]
        public void ADecreasingFeeIsStillCaught()
        {
            var script = Parse(WithUnreadLists(3, """
                DEFINE NUMBER ClaimCount AS 'Number of claims'
                BETWEEN 1 AND 100
                DEFAULT 10
                ENDDEFINE

                COMPUTE FEE Falling
                YIELD 1000 - 5 * ClaimCount
                ENDCOMPUTE

                VERIFY MONOTONIC FEE Falling WITH RESPECT TO ClaimCount
                RETURN Currency AS 'EUR'
                """));

            var result = new ScheduleVerifier().Verify(script.Inputs, script.Fees, script.Verifications);

            Assert.Empty(result.Unfinished);
            Assert.Single(result.MonotonicityFailures);
        }

        /// <summary>
        /// A monotonicity check that overruns the budget must not discard the completeness
        /// verdicts computed before it. The slow fee reads eight ten-way lists, so its check
        /// cannot finish in a second; the incomplete fee is still reported.
        /// </summary>
        [Fact]
        public void ACompletenessGapIsReportedEvenWhenAnotherDirectiveOverruns()
        {
            var lists = Enumerable.Range(1, 8).Select(i => $"L{i}").ToList();
            var source = new StringBuilder();
            foreach (var l in lists)
            {
                source.AppendLine($"DEFINE LIST {l} AS '{l}'");
                for (var c = 1; c <= 10; c++) source.AppendLine($"CHOICE {l}_{c} AS '{l} {c}'");
                source.AppendLine($"DEFAULT {l}_1");
                source.AppendLine("ENDDEFINE");
            }
            source.AppendLine("""
                DEFINE NUMBER ClaimCount AS 'Number of claims'
                BETWEEN 1 AND 1000
                DEFAULT 10
                ENDDEFINE

                DEFINE LIST EntitySize AS 'Entity size'
                CHOICE Entity_Large AS 'Large'
                CHOICE Entity_Small AS 'Small'
                DEFAULT Entity_Large
                ENDDEFINE

                COMPUTE FEE Gappy
                CASE EntitySize EQ Entity_Large AS
                YIELD 100
                ENDCASE
                ENDCOMPUTE
                """);
            source.AppendLine("COMPUTE FEE Slow");
            source.AppendLine("YIELD ClaimCount");
            foreach (var l in lists) source.AppendLine($"YIELD 1 IF {l} EQ {l}_2");
            source.AppendLine("ENDCOMPUTE");
            source.AppendLine("VERIFY COMPLETE FEE Gappy");
            source.AppendLine("VERIFY MONOTONIC FEE Slow WITH RESPECT TO ClaimCount");
            source.AppendLine("RETURN Currency AS 'EUR'");

            var script = Parse(source.ToString());
            var started = DateTime.UtcNow;
            var result = new ScheduleVerifier(new VerificationBudget { Limit = TimeSpan.FromSeconds(1) })
                .Verify(script.Inputs, script.Fees, script.Verifications);
            var elapsed = DateTime.UtcNow - started;

            Assert.Single(result.CompletenessFailures);
            Assert.Single(result.Unfinished);
            Assert.True(elapsed < TimeSpan.FromSeconds(5), $"budget not honoured: {elapsed.TotalSeconds:N1}s");
        }

        /// <summary>
        /// Every directive in the reference corpus finishes within the production budget, so the
        /// editor reports a verdict for each instead of saving schedules as unfinished.
        /// </summary>
        [Fact]
        public void EveryCorpusDirectiveFinishesWithinTheProductionBudget()
        {
            var verifier = new ScheduleVerifier(new VerificationBudget());
            var unfinished = new List<string>();
            var monotonicityChecked = 0;

            foreach (var code in JurisdictionCorpus.Codes)
            {
                var script = Compose(JurisdictionCorpus.GetCompositionChain(code));
                var result = verifier.Verify(script.Inputs, script.Fees, script.Verifications);
                unfinished.AddRange(result.Unfinished.Select(u => $"{code}: {u}"));
                monotonicityChecked += result.Monotonicity.Count;
            }

            Assert.True(unfinished.Count == 0, string.Join(Environment.NewLine, unfinished));
            Assert.Equal(63, monotonicityChecked);
        }

        /// <summary>
        /// A module change is refused when it breaks a definition that depended on it, and
        /// allowed when the definitions it reaches fail only as they already did.
        /// </summary>
        [Fact]
        public async Task AModuleChangeIsJudgedByWhatItDoesToItsDependents()
        {
            await new CorpusSeeder(fixture.JurisdictionRepository, fixture.FeeRepository, fixture.ModuleRepository).SeedAsync();

            var validator = new FeeDefinitionValidator(
                fixture.ModuleRepository,
                new FeeScriptComposer(new DslParser()),
                new DslCalculator(new DslParser()),
                new VerificationBudget(),
                fixture.FeeRepository);

            var baseEp = (await fixture.ModuleRepository.GetModules()).Single(m => m.Name.Equals("base_ep", StringComparison.OrdinalIgnoreCase));

            var harmless = await validator.ValidateModuleAsync(baseEp.Id, baseEp.SourceCode + Environment.NewLine + "# a comment", false);
            Assert.True(harmless.MayBeSaved, string.Join(" | ", harmless.Broken.SelectMany(b => b.NewFailures)));
            Assert.Equal(26, harmless.Dependents.Count);

            // Remove the claim-count input all 26 European members price against.
            var lines = baseEp.SourceCode.Split('\n').ToList();
            var start = lines.FindIndex(l => l.TrimStart().StartsWith("DEFINE NUMBER ClaimCount"));
            Assert.True(start >= 0, "base_ep declares ClaimCount");
            var end = lines.FindIndex(start, l => l.Trim() == "ENDDEFINE");
            lines.RemoveRange(start, end - start + 1);

            var breaking = await validator.ValidateModuleAsync(baseEp.Id, string.Join('\n', lines), false);
            Assert.False(breaking.MayBeSaved);
            Assert.NotEmpty(breaking.Broken);
        }

        private static string WithUnreadLists(int count, string rest)
        {
            var sb = new StringBuilder();
            for (var i = 1; i <= count; i++)
            {
                sb.AppendLine($"DEFINE LIST Unread{i} AS 'Unread {i}'");
                for (var c = 1; c <= 10; c++) sb.AppendLine($"CHOICE U{i}_{c} AS 'U{i} {c}'");
                sb.AppendLine($"DEFAULT U{i}_1");
                sb.AppendLine("ENDDEFINE");
            }
            sb.AppendLine(rest);
            return sb.ToString();
        }

        private static ParsedScript Compose(IReadOnlyList<(string Id, string Source)> chain)
        {
            var parser = new DslParser();
            var registry = new JurisdictionRegistry();
            string? parent = null, leaf = null;
            foreach (var (id, source) in chain)
            {
                var (parsed, errors) = parser.Parse(source, returnParsedScript: true);
                Assert.True(parsed is not null, $"{id}: {string.Join(" | ", errors)}");
                registry.Register(new Jurisdiction(id, id, parsed!, parent));
                parent = id;
                leaf = id;
            }
            return new JurisdictionComposer(registry).Compose(leaf!).Script;
        }
    }
}
