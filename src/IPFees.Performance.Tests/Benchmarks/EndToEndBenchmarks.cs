using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IPFees.Calculator;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Performance.Tests.Fixtures;
using System.Diagnostics;

namespace IPFees.Performance.Tests.Benchmarks
{
    /// <summary>
    /// End-to-end benchmarks simulating real-world usage patterns
    /// Measures complete workflow from input validation to result generation
    /// </summary>
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class EndToEndBenchmarks : IAsyncDisposable
    {
        private BenchmarkFixture fixture = null!;
        private IJurisdictionFeeManager feeManager = null!;
        private FeeCalculator feeCalculator = null!;

        [GlobalSetup]
        public async Task Setup()
        {
            fixture = new BenchmarkFixture();
            await fixture.InitializeTestDataAsync();

            var parser = new DslParser();
            IDslCalculator calculator = new DslCalculator(parser);
            feeCalculator = new FeeCalculator(fixture.FeeRepository, fixture.ModuleRepository, calculator);
            
            var currencyConverter = new MockCurrencyConverter();
            
            feeManager = new JurisdictionFeeManager(
                feeCalculator,
                fixture.FeeRepository,
                fixture.JurisdictionRepository,
                fixture.SettingsRepository,
                currencyConverter);
        }

        [GlobalCleanup]
        public async Task Cleanup()
        {
            await fixture.DisposeAsync();
        }

        /// <summary>
        /// Simulates a typical user workflow: EPO regional phase entry
        /// Target: <200ms
        /// </summary>
        [Benchmark(Description = "E2E: EPO Regional Phase Entry (Typical)")]
        public async Task<TotalFeeInfo> EPOTypicalCase()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 20),
                new IPFValueBoolean("Examination", true)
            };

            return await feeManager.Calculate(
                new[] { "TEST_COMPLEX" },
                parameters,
                "EUR",
                0);
        }

        /// <summary>
        /// Simulates multi-jurisdiction portfolio calculation
        /// This is the CRITICAL benchmark for the <500ms claim
        /// Target: <500ms
        /// </summary>
        [Benchmark(Description = "E2E: Multi-Jurisdiction Portfolio (EP+US+JP equivalent)")]
        public async Task<TotalFeeInfo> MultiJurisdictionPortfolio()
        {
            // Get consolidated inputs first (simulating UI behavior)
            var (inputs, groups, errors) = feeManager.GetConsolidatedInputs(
                new[] { "TEST_SIMPLE", "TEST_MEDIUM", "TEST_COMPLEX" });

            // Prepare parameters
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 45),
                new IPFValueNumber("ClaimCount", 22),
                new IPFValueBoolean("Examination", true)
            };

            // Calculate fees with currency conversion
            return await feeManager.Calculate(
                new[] { "TEST_SIMPLE", "TEST_MEDIUM", "TEST_COMPLEX" },
                parameters,
                "USD",
                0);
        }

        /// <summary>
        /// Simulates a high-complexity scenario with many claims
        /// Target: <750ms (acceptable for P99)
        /// </summary>
        [Benchmark(Description = "E2E: High Complexity Case (Many Claims)")]
        public async Task<TotalFeeInfo> HighComplexityCase()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueString("ISA", "ISA_OTHER"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 120),
                new IPFValueNumber("ClaimCount", 95),
                new IPFValueBoolean("Examination", true)
            };

            return await feeManager.Calculate(
                new[] { "TEST_COMPLEX" },
                parameters,
                "EUR",
                0);
        }

        /// <summary>
        /// Simulates rapid successive calculations (caching test)
        /// </summary>
        [Benchmark(Description = "E2E: Rapid Successive Calculations (5x)")]
        public async Task<List<TotalFeeInfo>> RapidSuccessiveCalculations()
        {
            var results = new List<TotalFeeInfo>();
            
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 20),
                new IPFValueBoolean("Examination", true)
            };

            for (int i = 0; i < 5; i++)
            {
                results.Add(await feeManager.Calculate(
                    new[] { "TEST_COMPLEX" },
                    parameters,
                    "EUR",
                    0));
            }

            return results;
        }

        /// <summary>
        /// Simulates varying parameters across calculations (no cache benefit)
        /// </summary>
        [Benchmark(Description = "E2E: Varied Parameter Calculations (5x)")]
        public async Task<List<TotalFeeInfo>> VariedParameterCalculations()
        {
            var results = new List<TotalFeeInfo>();

            for (int sheetCount = 35; sheetCount <= 75; sheetCount += 10)
            {
                var parameters = new List<IPFValue>
                {
                    new IPFValueString("ISA", "ISA_EPO"),
                    new IPFValueString("IPRP", "IPRP_NONE"),
                    new IPFValueNumber("SheetCount", sheetCount),
                    new IPFValueNumber("ClaimCount", 15 + (sheetCount / 10)),
                    new IPFValueBoolean("Examination", true)
                };

                results.Add(await feeManager.Calculate(
                    new[] { "TEST_COMPLEX" },
                    parameters,
                    "EUR",
                    0));
            }

            return results;
        }

        /// <summary>
        /// Complete workflow: input retrieval + validation + calculation
        /// Simulates the full API request cycle
        /// Target: <500ms total
        /// </summary>
        [Benchmark(Description = "E2E: Complete API Workflow (Input + Calc)")]
        public async Task<(object inputs, TotalFeeInfo result)> CompleteAPIWorkflow()
        {
            // Step 1: Get inputs (like /api/v1/Fee/Parameters endpoint)
            var inputs = feeManager.GetConsolidatedInputs(
                new[] { "TEST_COMPLEX" });

            // Step 2: Prepare parameters based on inputs
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 20),
                new IPFValueBoolean("Examination", true)
            };

            // Step 3: Calculate (like /api/v1/Fee/Calculate endpoint)
            var result = await feeManager.Calculate(
                new[] { "TEST_COMPLEX" },
                parameters,
                "EUR",
                0);

            return (inputs, result);
        }

        public async ValueTask DisposeAsync()
        {
            if (fixture != null)
            {
                await fixture.DisposeAsync();
            }
        }
    }
}
