using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IPFees.Calculator;
using IPFees.Core.FeeCalculation;
using IPFees.Core.FeeManager;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Performance.Tests.Fixtures;

namespace IPFees.Performance.Tests.Benchmarks
{
    /// <summary>
    /// Benchmarks for JurisdictionFeeManager - multi-jurisdiction calculations
    /// Target: Validate sub-500ms for complex multi-jurisdiction scenarios (PRIMARY VALIDATION)
    /// </summary>
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class JurisdictionFeeManagerBenchmarks : IAsyncDisposable
    {
        private BenchmarkFixture fixture = null!;
        private IJurisdictionFeeManager feeManager = null!;

        [GlobalSetup]
        public async Task Setup()
        {
            fixture = new BenchmarkFixture();
            await fixture.InitializeTestDataAsync();

            var parser = new DslParser();
            IDslCalculator calculator = new DslCalculator(parser);
            var feeCalculator = new FeeCalculator(fixture.FeeRepository, fixture.ModuleRepository, calculator);
            
            // Create a mock currency converter (no actual API calls in benchmarks)
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

        [Benchmark(Description = "Single Jurisdiction - Simple")]
        public async Task<TotalFeeInfo> SingleJurisdictionSimple()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueNumber("SheetCount", 35)
            };

            return await feeManager.Calculate(
                new[] { "TEST_SIMPLE" },
                parameters,
                "USD",
                0);
        }

        [Benchmark(Description = "Single Jurisdiction - Medium")]
        public async Task<TotalFeeInfo> SingleJurisdictionMedium()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 15)
            };

            return await feeManager.Calculate(
                new[] { "TEST_MEDIUM" },
                parameters,
                "EUR",
                0);
        }

        [Benchmark(Description = "Single Jurisdiction - Complex (EPO-like)")]
        public async Task<TotalFeeInfo> SingleJurisdictionComplex()
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

        [Benchmark(Description = "Multi-Jurisdiction - 2 Jurisdictions")]
        public async Task<TotalFeeInfo> TwoJurisdictions()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 15)
            };

            return await feeManager.Calculate(
                new[] { "TEST_SIMPLE", "TEST_MEDIUM" },
                parameters,
                "USD",
                0);
        }

        [Benchmark(Description = "Multi-Jurisdiction - 3 Jurisdictions (Standard Portfolio)")]
        public async Task<TotalFeeInfo> ThreeJurisdictions()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 20),
                new IPFValueBoolean("Examination", true)
            };

            return await feeManager.Calculate(
                new[] { "TEST_SIMPLE", "TEST_MEDIUM", "TEST_COMPLEX" },
                parameters,
                "USD",
                0);
        }

        [Benchmark(Description = "Get Consolidated Inputs - Single Jurisdiction")]
        public (IEnumerable<DslInput>, IEnumerable<DslGroup>, IEnumerable<FeeResultFail>) GetInputsSingle()
        {
            return feeManager.GetConsolidatedInputs(new[] { "TEST_COMPLEX" });
        }

        [Benchmark(Description = "Get Consolidated Inputs - Multiple Jurisdictions")]
        public (IEnumerable<DslInput>, IEnumerable<DslGroup>, IEnumerable<FeeResultFail>) GetInputsMultiple()
        {
            return feeManager.GetConsolidatedInputs(new[] { "TEST_SIMPLE", "TEST_MEDIUM", "TEST_COMPLEX" });
        }

        [Benchmark(Description = "CRITICAL: Complex Multi-Jurisdiction Calculation (Sub-500ms Target)")]
        public async Task<TotalFeeInfo> ComplexMultiJurisdictionTarget()
        {
            // This simulates the critical path: multi-jurisdiction with complex parameters
            // This is the PRIMARY benchmark to validate the <500ms claim
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueString("ISA", "ISA_OTHER"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 55),
                new IPFValueNumber("ClaimCount", 30),
                new IPFValueBoolean("Examination", true)
            };

            // Calculate across all three test jurisdictions with currency conversion
            return await feeManager.Calculate(
                new[] { "TEST_SIMPLE", "TEST_MEDIUM", "TEST_COMPLEX" },
                parameters,
                "USD",
                0);
        }

        [Benchmark(Description = "STRESS TEST: Worst Case Multi-Jurisdiction")]
        public async Task<TotalFeeInfo> WorstCaseMultiJurisdiction()
        {
            // Worst case scenario: complex parameters, high values
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueString("ISA", "ISA_OTHER"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 100),
                new IPFValueNumber("ClaimCount", 85),
                new IPFValueBoolean("Examination", true)
            };

            return await feeManager.Calculate(
                new[] { "TEST_SIMPLE", "TEST_MEDIUM", "TEST_COMPLEX" },
                parameters,
                "USD",
                0);
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
