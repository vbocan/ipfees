using IPFees.Core.FeeCalculation;
using IPFees.Core.Tests.Fixture;
using IPFLang.Engine;
using IPFLang.Parser;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// The editor refuses a fee definition that the IPFLang engine rejects. These tests pin
    /// down what "rejects" covers: syntax, currency typing, and the VERIFY directives the
    /// author declared for themselves.
    /// </summary>
    public class FeeDefinitionValidatorTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public FeeDefinitionValidatorTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        private FeeDefinitionValidator NewValidator()
        {
            var parser = new DslParser();
            return new FeeDefinitionValidator(
                fixture.ModuleRepository,
                new FeeScriptComposer(parser),
                new DslCalculator(new DslParser()));
        }

        [Fact]
        public async Task WellFormedDefinitionCanBeStored()
        {
            var source =
            """
            COMPUTE FEE FilingFee
            YIELD 320
            ENDCOMPUTE
            """;

            var report = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());

            Assert.Empty(report.ParseErrors);
            Assert.Empty(report.TypeErrors);
            Assert.True(report.CanBeStored);
        }

        [Fact]
        public async Task DefinitionWithoutVerifyDirectivesIsReportedAsUnchecked()
        {
            var source =
            """
            COMPUTE FEE FilingFee
            YIELD 320
            ENDCOMPUTE
            """;

            var report = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());

            // Storable, but the author should know nothing was statically verified.
            Assert.True(report.CanBeStored);
            Assert.True(report.HasNoVerifications);
            Assert.Equal(0, report.VerificationsRun);
        }

        [Fact]
        public async Task SyntaxErrorBlocksStorage()
        {
            var source =
            """
            COMPUTE FEE FilingFee
            THIS IS NOT IPFLANG
            ENDCOMPUTE
            """;

            var report = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());

            Assert.NotEmpty(report.ParseErrors);
            Assert.False(report.CanBeStored);
        }

        [Fact]
        public async Task EmptyDefinitionBlocksStorage()
        {
            var report = await NewValidator().ValidateAsync("   ", Array.Empty<Guid>());

            Assert.NotEmpty(report.ParseErrors);
            Assert.False(report.CanBeStored);
        }

        [Fact]
        public async Task IncompleteCoverageBlocksStorageWhenVerifyCompleteIsDeclared()
        {
            // EntityType has three choices but only two are answered, and the definition
            // asks IPFLang to check exactly that.
            var source =
            """
            DEFINE LIST EntityType AS 'Entity type'
            CHOICE LargeEntity AS 'Large'
            CHOICE SmallEntity AS 'Small'
            CHOICE MicroEntity AS 'Micro'
            DEFAULT LargeEntity
            ENDDEFINE

            COMPUTE FEE FilingFee
            CASE EntityType EQ LargeEntity AS
            YIELD 320
            ENDCASE
            CASE EntityType EQ SmallEntity AS
            YIELD 160
            ENDCASE
            ENDCOMPUTE

            VERIFY COMPLETE FEE FilingFee
            """;

            var report = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());

            Assert.True(report.CanBeStored, "the script itself is well formed");
            Assert.NotEmpty(report.CompletenessFailures);
            Assert.False(report.VerificationsPassed);
            Assert.Contains(report.CompletenessFailures, f => f.Contains("FilingFee"));
        }

        [Fact]
        public async Task CompleteCoveragePassesWhenVerifyCompleteIsDeclared()
        {
            var source =
            """
            DEFINE LIST EntityType AS 'Entity type'
            CHOICE LargeEntity AS 'Large'
            CHOICE SmallEntity AS 'Small'
            DEFAULT LargeEntity
            ENDDEFINE

            COMPUTE FEE FilingFee
            CASE EntityType EQ LargeEntity AS
            YIELD 320
            ENDCASE
            CASE EntityType EQ SmallEntity AS
            YIELD 160
            ENDCASE
            ENDCOMPUTE

            VERIFY COMPLETE FEE FilingFee
            """;

            var report = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());

            Assert.Empty(report.CompletenessFailures);
            Assert.True(report.VerificationsPassed);
            Assert.True(report.IsClean);
            Assert.Equal(1, report.VerificationsRun);
        }

        [Fact]
        public async Task DefinitionInheritsInputsFromAReferencedModule()
        {
            // The module declares the input; the fee uses it. Neither parses alone, which is
            // the whole point of composing them before validation.
            var module = await fixture.ModuleRepository.AddModuleAsync($"ClaimCount_{Guid.NewGuid():N}");
            Assert.True(module.Success);

            var moduleSource =
            """
            DEFINE NUMBER ClaimCount AS 'Number of claims'
            BETWEEN 1 AND 100
            DEFAULT 10
            ENDDEFINE
            """;
            Assert.True((await fixture.ModuleRepository.SetModuleSourceCodeAsync(module.Id, moduleSource)).Success);

            var source =
            """
            COMPUTE FEE ExcessClaimsFee
            YIELD 25 * ClaimCount
            ENDCOMPUTE
            """;

            var withModule = await NewValidator().ValidateAsync(source, new[] { module.Id });
            Assert.True(withModule.CanBeStored);

            var withoutModule = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());
            Assert.False(withoutModule.CanBeStored);
        }

        [Fact]
        public async Task LineEndingsDoNotAffectValidation()
        {
            // A definition may be pasted from any editor on any platform, or arrive over the
            // API. All three conventions must give the same verdict.
            const string crlf = "COMPUTE FEE FilingFee\r\nYIELD 320\r\nENDCOMPUTE";
            const string lf = "COMPUTE FEE FilingFee\nYIELD 320\nENDCOMPUTE";
            const string cr = "COMPUTE FEE FilingFee\rYIELD 320\rENDCOMPUTE";

            foreach (var source in new[] { crlf, lf, cr })
            {
                var report = await NewValidator().ValidateAsync(source, Array.Empty<Guid>());
                Assert.True(report.CanBeStored, $"failed for {source.Replace("\r", "\\r").Replace("\n", "\\n")}");
            }
        }
    }
}
