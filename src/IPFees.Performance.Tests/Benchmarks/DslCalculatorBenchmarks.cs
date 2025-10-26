using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;

namespace IPFees.Performance.Tests.Benchmarks
{
    /// <summary>
    /// Benchmarks for the DSL Calculator - core parsing and execution engine
    /// Target: Validate sub-100ms for complex DSL parsing and execution
    /// </summary>
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DslCalculatorBenchmarks
    {
        private DslParser parser = null!;
        private IDslCalculator calculator = null!;
        
        private string simpleScript = string.Empty;
        private string mediumScript = string.Empty;
        private string complexScript = string.Empty;

        [GlobalSetup]
        public void Setup()
        {
            parser = new DslParser();
            calculator = new DslCalculator(parser);

            // Simple script - baseline
            simpleScript = """
                COMPUTE FEE SimpleFee
                YIELD 100
                ENDCOMPUTE
                """;

            // Medium complexity script
            mediumScript = """
                DEFINE NUMBER SheetCount AS 'Number of sheets'
                BETWEEN 1 AND 1000
                DEFAULT 30
                ENDDEFINE
                
                DEFINE NUMBER ClaimCount AS 'Number of claims'
                BETWEEN 1 AND 100
                DEFAULT 10
                ENDDEFINE
                
                COMPUTE FEE BasicFee
                YIELD 400
                ENDCOMPUTE
                
                COMPUTE FEE SheetFee
                LET Fee AS 10
                YIELD Fee * (SheetCount - 30) IF SheetCount GT 30
                ENDCOMPUTE
                
                COMPUTE FEE ClaimFee
                LET Fee AS 50
                YIELD Fee * (ClaimCount - 20) IF ClaimCount GT 20
                ENDCOMPUTE
                """;

            // Complex script (EPO-like)
            complexScript = """
                DEFINE LIST ISA AS 'International Search Authority'
                CHOICE ISA_EPO AS 'EPO'
                CHOICE ISA_USPTO AS 'USPTO'
                CHOICE ISA_JPO AS 'JPO'
                CHOICE ISA_OTHER AS 'Other'
                DEFAULT ISA_EPO
                ENDDEFINE
                
                DEFINE LIST IPRP AS 'International Preliminary Report'
                CHOICE IPRP_EPO AS 'EPO'
                CHOICE IPRP_NONE AS 'None'
                DEFAULT IPRP_NONE
                ENDDEFINE
                
                DEFINE NUMBER SheetCount AS 'Number of sheets'
                BETWEEN 1 AND 1000
                DEFAULT 35
                ENDDEFINE
                
                DEFINE NUMBER ClaimCount AS 'Number of claims'
                BETWEEN 1 AND 100
                DEFAULT 15
                ENDDEFINE
                
                DEFINE BOOLEAN Examination AS 'Request examination'
                DEFAULT true
                ENDDEFINE
                
                COMPUTE FEE BasicNationalFee
                YIELD 135
                ENDCOMPUTE
                
                COMPUTE FEE DesignationFee
                YIELD 660
                ENDCOMPUTE
                
                COMPUTE FEE SheetFee
                LET Fee AS 17
                YIELD Fee * (SheetCount - 35) IF SheetCount GT 35
                ENDCOMPUTE
                
                COMPUTE FEE ClaimFee
                LET StandardFee AS 265
                LET OverLimitFee AS 665
                LET ExcessClaims AS ClaimCount - 15
                LET Over50 AS ClaimCount - 50
                
                YIELD (StandardFee * ExcessClaims) IF (ClaimCount GT 15) AND (ClaimCount LE 50)
                YIELD (StandardFee * 35) + (OverLimitFee * Over50) IF ClaimCount GT 50
                ENDCOMPUTE
                
                COMPUTE FEE SearchFee
                LET FullFee AS 1430
                LET ReducedFee AS 143
                
                YIELD 0 IF ISA EQ ISA_EPO
                YIELD ReducedFee IF (ISA EQ ISA_USPTO) OR (ISA EQ ISA_JPO)
                YIELD FullFee IF ISA EQ ISA_OTHER
                ENDCOMPUTE
                
                COMPUTE FEE ExaminationFee
                LET Fee AS 2055
                YIELD Fee IF Examination EQ true
                ENDCOMPUTE
                """;
        }

        [Benchmark(Description = "Parse Simple Script")]
        public bool ParseSimple()
        {
            calculator.Reset();
            return calculator.Parse(simpleScript);
        }

        [Benchmark(Description = "Parse Medium Complexity Script")]
        public bool ParseMedium()
        {
            calculator.Reset();
            return calculator.Parse(mediumScript);
        }

        [Benchmark(Description = "Parse Complex Script (EPO-like)")]
        public bool ParseComplex()
        {
            calculator.Reset();
            return calculator.Parse(complexScript);
        }

        [Benchmark(Description = "Parse + Execute Simple")]
        public (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) ExecuteSimple()
        {
            calculator.Reset();
            calculator.Parse(simpleScript);
            return calculator.Compute(new List<IPFValue>());
        }

        [Benchmark(Description = "Parse + Execute Medium with Parameters")]
        public (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) ExecuteMedium()
        {
            calculator.Reset();
            calculator.Parse(mediumScript);
            
            var parameters = new List<IPFValue>
            {
                new IPFValueNumber("SheetCount", 45),
                new IPFValueNumber("ClaimCount", 25)
            };
            
            return calculator.Compute(parameters);
        }

        [Benchmark(Description = "Parse + Execute Complex with Full Parameters")]
        public (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) ExecuteComplex()
        {
            calculator.Reset();
            calculator.Parse(complexScript);
            
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_USPTO"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 50),
                new IPFValueNumber("ClaimCount", 25),
                new IPFValueBoolean("Examination", true)
            };
            
            return calculator.Compute(parameters);
        }

        [Benchmark(Description = "Execute Complex - Worst Case (Many Claims)")]
        public (decimal, decimal, IEnumerable<string>, IEnumerable<(string, string)>) ExecuteComplexWorstCase()
        {
            calculator.Reset();
            calculator.Parse(complexScript);
            
            var parameters = new List<IPFValue>
            {
                new IPFValueString("ISA", "ISA_OTHER"),
                new IPFValueString("IPRP", "IPRP_NONE"),
                new IPFValueNumber("SheetCount", 100),
                new IPFValueNumber("ClaimCount", 75),
                new IPFValueBoolean("Examination", true)
            };
            
            return calculator.Compute(parameters);
        }
    }
}
