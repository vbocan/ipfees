using IPFees.Core.Enum;
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

        private CorpusSeeder NewSeeder(Func<IFeeDefinitionValidator, IFeeDefinitionValidator>? wrap = null)
        {
            IFeeDefinitionValidator validator = new FeeDefinitionValidator(fixture.ModuleRepository, new FeeScriptComposer(new DslParser()), new DslCalculator(new DslParser()), new VerificationBudget());
            return new(fixture.JurisdictionRepository, fixture.FeeRepository, fixture.ModuleRepository, wrap is null ? validator : wrap(validator));
        }

        /// <summary>
        /// Refuses one jurisdiction's schedule as if a directive had failed. Every schedule in the
        /// corpus passes the gate, so this is how the seeder's handling of a refusal is exercised.
        /// </summary>
        private sealed class RefusingValidator(IFeeDefinitionValidator inner, string code) : IFeeDefinitionValidator
        {
            public async Task<FeeValidationReport> ValidateAsync(string sourceCode, IEnumerable<Guid> referencedModules)
            {
                var report = await inner.ValidateAsync(sourceCode, referencedModules);
                return sourceCode.Contains($"RETURN Jurisdiction AS '{code}'")
                    ? report with { CompletenessFailures = report.CompletenessFailures.Append($"'{code}' refused by the test").ToList() }
                    : report;
            }

            public Task<ModuleValidationReport> ValidateModuleAsync(Guid? moduleId, string sourceCode, bool autoRun) =>
                inner.ValidateModuleAsync(moduleId, sourceCode, autoRun);

            public Task<ModuleValidationReport> ValidateModuleRemovalAsync(Guid moduleId) =>
                inner.ValidateModuleRemovalAsync(moduleId);
        }

        private FeeCalculator NewCalculator(VerificationBudget? budget = null) =>
            new(fixture.FeeRepository,
                fixture.ModuleRepository,
                new DslCalculator(new DslParser()),
                new FeeScriptComposer(new DslParser()),
                budget);

        [Fact]
        public async Task SeedingLeavesExactlyOneSchedulePerJurisdiction()
        {
            // IPFees used to hold three documents per jurisdiction, split by category. A
            // calculation sums every definition registered for a jurisdiction, so if the old
            // documents survive alongside the corpus schedule every fee is counted twice.
            var stale = await fixture.FeeRepository.AddFeeAsync("PCT-RO-OFF");
            Assert.True(stale.Success);
            Assert.True((await fixture.FeeRepository.SetFeeJurisdictionNameAsync(stale.Id, "RO")).Success);
            Assert.True((await fixture.FeeRepository.SetFeeSourceCodeAsync(stale.Id, "COMPUTE FEE Legacy\nYIELD 1\nENDCOMPUTE")).Success);

            var report = await NewSeeder().SeedAsync();

            Assert.True(report.Succeeded, string.Join(" | ", report.Errors));
            Assert.True(report.Superseded >= 1, "the legacy document should have been removed");

            var romanian = (await fixture.FeeRepository.GetFees())
                .Where(f => f.JurisdictionName.Equals("RO", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var only = Assert.Single(romanian);
            Assert.Equal("PCT-RO", only.Name);
        }

        /// <summary>
        /// The corpus passes the same gate as an author's save, and every schedule in it passes.
        /// </summary>
        [Fact]
        public async Task SeedsEveryJurisdictionTheGateAcceptsAndEveryBase()
        {
            var report = await NewSeeder().SeedAsync();

            Assert.True(report.Succeeded, string.Join(" | ", report.Errors));
            Assert.Empty(report.RefusedSchedules);
            Assert.Equal(JurisdictionCorpus.Codes.Count, report.Jurisdictions);
            Assert.Equal(JurisdictionCorpus.BaseNames.Count, report.Bases);

            var stored = await fixture.FeeRepository.GetFees();
            foreach (var code in JurisdictionCorpus.Codes)
            {
                Assert.Contains(stored, f => f.Name.Equals($"PCT-{code}", StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// A schedule the gate refuses is not served, and neither is a copy of it left from an
        /// earlier load.
        /// </summary>
        [Fact]
        public async Task ARefusedScheduleIsNotServedAndAnEarlierCopyIsRemoved()
        {
            var earlier = await fixture.FeeRepository.AddFeeAsync("PCT-CN");
            Assert.True(earlier.Success);
            Assert.True((await fixture.FeeRepository.SetFeeJurisdictionNameAsync(earlier.Id, "CN")).Success);
            Assert.True((await fixture.FeeRepository.SetFeeSourceCodeAsync(earlier.Id, JurisdictionCorpus.GetJurisdiction("CN"))).Success);

            var report = await NewSeeder(inner => new RefusingValidator(inner, "CN")).SeedAsync();

            var refused = Assert.Single(report.RefusedSchedules);
            Assert.Equal("CN", refused.Code);
            Assert.Equal(JurisdictionCorpus.Codes.Count - 1, report.Jurisdictions);
            Assert.DoesNotContain(await fixture.FeeRepository.GetFees(), f => f.Name.Equals("PCT-CN", StringComparison.OrdinalIgnoreCase));

            // Seeding again with the real gate serves it once more.
            await NewSeeder().SeedAsync();
            Assert.Contains(await fixture.FeeRepository.GetFees(), f => f.Name.Equals("PCT-CN", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// The public instance is a sandbox anyone may edit, and its reset returns it to the state
        /// a fresh deployment starts in: what a visitor added goes, and what a visitor changed in
        /// the reference set is put back. A copy whose name differs from a reference schedule's
        /// only in case goes too, rather than being taken for the reference one.
        /// </summary>
        [Fact]
        public async Task ResettingReturnsTheStoreToTheCorpus()
        {
            await NewSeeder().SeedAsync();

            var added = await fixture.JurisdictionRepository.AddJurisdictionAsync("ZZ");
            Assert.True(added.Success);
            var schedule = await fixture.FeeRepository.AddFeeAsync("PCT-ZZ");
            Assert.True((await fixture.FeeRepository.SetFeeJurisdictionNameAsync(schedule.Id, "ZZ")).Success);
            var module = await fixture.ModuleRepository.AddModuleAsync("visitor_surcharge");
            Assert.True((await fixture.ModuleRepository.SetModuleAutoRunStatusAsync(module.Id, true)).Success);
            var lookalike = await fixture.FeeRepository.AddFeeAsync("pct-ro");
            Assert.True((await fixture.FeeRepository.SetFeeJurisdictionNameAsync(lookalike.Id, "RO")).Success);

            var romania = (await fixture.FeeRepository.GetFees()).Single(f => f.Name == "PCT-RO");
            Assert.True((await fixture.FeeRepository.SetFeeSourceCodeAsync(romania.Id, "COMPUTE FEE Edited\nYIELD 1\nENDCOMPUTE")).Success);
            var ro = await fixture.JurisdictionRepository.GetJurisdictionByName("RO");
            Assert.True((await fixture.JurisdictionRepository.SetJurisdictionServiceFeeLevelAsync(ro.Id, ServiceFeeLevel.Level3)).Success);

            var report = await NewSeeder().ResetAsync();

            Assert.True(report.Succeeded, string.Join(" | ", report.Errors.Concat(report.Seed.Errors)));
            Assert.True(report.Definitions >= 2, "PCT-ZZ and pct-ro should have been removed");
            Assert.True(report.Modules >= 1, "the visitor's module should have been removed");
            Assert.True(report.Jurisdictions >= 1, "ZZ should have been removed");

            var stored = (await fixture.FeeRepository.GetFees()).ToList();
            Assert.Equal(
                JurisdictionCorpus.Codes.Select(c => $"PCT-{c}").OrderBy(n => n, StringComparer.Ordinal),
                stored.Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal));
            Assert.Equal(JurisdictionCorpus.GetJurisdiction("RO"), stored.Single(f => f.Name == "PCT-RO").SourceCode);
            Assert.Single(stored, f => f.JurisdictionName.Equals("RO", StringComparison.OrdinalIgnoreCase));

            Assert.Equal(
                JurisdictionCorpus.BaseNames.OrderBy(n => n, StringComparer.Ordinal),
                (await fixture.ModuleRepository.GetModules()).Select(m => m.Name).OrderBy(n => n, StringComparer.Ordinal));

            var jurisdictions = (await fixture.JurisdictionRepository.GetJurisdictions()).ToList();
            Assert.Equal(
                JurisdictionCorpus.Codes.OrderBy(n => n, StringComparer.Ordinal),
                jurisdictions.Select(j => j.Name).OrderBy(n => n, StringComparer.Ordinal));
            Assert.All(jurisdictions, j => Assert.Equal(ServiceFeeLevel.Level1, j.ServiceFeeLevel));
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
        /// hold.
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
            Assert.Empty(verification.Results.Unfinished);
            Assert.Empty(verification.Results.Errors);
            Assert.Empty(verification.Results.CompletenessFailures);
            Assert.Empty(verification.Results.MonotonicityFailures);
            Assert.True(elapsed < TimeSpan.FromSeconds(10), $"{name} took {elapsed.TotalSeconds:N1}s to verify");
        }

        /// <summary>
        /// The REST endpoint that runs verification is reachable without credentials, so it must
        /// answer within the budget. PCT-US declares monotonicity directives, which overran any
        /// budget while the checker was handed every declared input; given only the inputs each
        /// fee reads, they finish and are reported.
        /// </summary>
        [Fact]
        public async Task VerifyingAWideScheduleFinishesWithinTheBudget()
        {
            await NewSeeder().SeedAsync();

            var fee = (await fixture.FeeRepository.GetFees())
                .Single(f => f.Name.Equals("PCT-US", StringComparison.OrdinalIgnoreCase));

            var calculator = NewCalculator(new VerificationBudget { Limit = TimeSpan.FromSeconds(5) });

            var started = DateTime.UtcNow;
            var result = calculator.Verify(fee.Id);
            var elapsed = DateTime.UtcNow - started;

            var verification = Assert.IsType<FeeResultVerification>(result);
            Assert.False(verification.TimedOut, string.Join(" | ", verification.Results.Unfinished));
            Assert.NotEmpty(verification.Results.Monotonicity);
            Assert.True(elapsed < TimeSpan.FromSeconds(5), $"took {elapsed.TotalSeconds:N1}s");
        }

        [Fact]
        public async Task MonotonicityCheckingFinishesOnWideSchedules()
        {
            var validator = new FeeDefinitionValidator(
                fixture.ModuleRepository,
                new FeeScriptComposer(new DslParser()),
                new DslCalculator(new DslParser()),
                new VerificationBudget { Limit = TimeSpan.FromSeconds(5) });

            var report = await validator.ValidateAsync(JurisdictionCorpus.GetJurisdiction("US"), Array.Empty<Guid>());

            Assert.True(report.CanBeStored, "the schedule itself is well formed");
            Assert.False(report.VerificationTimedOut, string.Join(" | ", report.UnfinishedDirectives));
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
