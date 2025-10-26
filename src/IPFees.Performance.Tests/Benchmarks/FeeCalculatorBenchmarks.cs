using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IPFees.Calculator;
using IPFees.Core.FeeCalculation;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Performance.Tests.Fixtures;

namespace IPFees.Performance.Tests.Benchmarks
{
    /// <summary>
    /// Benchmarks for the Fee Calculator - business logic layer with database integration
    /// Target: Validate sub-200ms for single jurisdiction calculations with DB access
    /// </summary>
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class FeeCalculatorBenchmarks : IAsyncDisposable
    {
        private BenchmarkFixture fixture = null!;
        private FeeCalculator feeCalculator = null!;
        private Guid simpleFeeId;
        private Guid mediumFeeId;
        private Guid complexFeeId;

        [GlobalSetup]
        public async Task Setup()
        {
            fixture = new BenchmarkFixture();
            await fixture.InitializeTestDataAsync();

            var parser = new DslParser();
            IDslCalculator calculator = new DslCalculator(parser);
            feeCalculator = new FeeCalculator(fixture.FeeRepository, fixture.ModuleRepository, calculator);

            // Get fee IDs by name
            var fees = await fixture.FeeRepository.GetFees();
            simpleFeeId = fees.FirstOrDefault(f => f.Name == "TEST_SIMPLE")?.Id ?? Guid.Empty;
            mediumFeeId = fees.FirstOrDefault(f => f.Name == "TEST_MEDIUM")?.Id ?? Guid.Empty;
            complexFeeId = fees.FirstOrDefault(f => f.Name == "TEST_COMPLEX")?.Id ?? Guid.Empty;
        }

        [GlobalCleanup]
        public async Task Cleanup()
        {
            await fixture.DisposeAsync();
        }

        [Benchmark(Description = "Calculate Simple Fee (Baseline)")]
        public FeeResult CalculateSimpleFee()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueNumber("SheetCount", 35)
            };

            return feeCalculator.Calculate(simpleFeeId, parameters);
        }

        [Benchmark(Description = "Calculate Medium Fee (Standard Case)")]
        public FeeResult CalculateMediumFee()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 15)
            };

            return feeCalculator.Calculate(mediumFeeId, parameters);
        }

        [Benchmark(Description = "Calculate Complex Fee (EPO-like Standard)")]
        public FeeResult CalculateComplexFee()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 20),
                new IPFValueBoolean("Examination", true)
            };

            return feeCalculator.Calculate(complexFeeId, parameters);
        }

        [Benchmark(Description = "Calculate Complex Fee (Worst Case Scenario)")]
        public FeeResult CalculateComplexFeeWorstCase()
        {
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_OTHER"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 100),
                new IPFValueNumber("ClaimCount", 80),
                new IPFValueBoolean("Examination", true)
            };

            return feeCalculator.Calculate(complexFeeId, parameters);
        }

        [Benchmark(Description = "Get Inputs for Complex Fee")]
        public FeeResult GetComplexFeeInputs()
        {
            return feeCalculator.GetInputs(complexFeeId);
        }

        [Benchmark(Description = "Calculate All Three Jurisdictions Sequentially")]
        public List<FeeResult> CalculateMultipleSequential()
        {
            var results = new List<FeeResult>();

            // Simple
            var params1 = new List<IPFValue>
            {
                new IPFValueNumber("SheetCount", 35)
            };
            results.Add(feeCalculator.Calculate(simpleFeeId, params1));

            // Medium
            var params2 = new List<IPFValue>
            {
                new IPFValueString("EntitySize", "Entity_Company"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 15)
            };
            results.Add(feeCalculator.Calculate(mediumFeeId, params2));

            // Complex
            var params3 = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_EPO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 40),
                new IPFValueNumber("ClaimCount", 20),
                new IPFValueBoolean("Examination", true)
            };
            results.Add(feeCalculator.Calculate(complexFeeId, params3));

            return results;
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
