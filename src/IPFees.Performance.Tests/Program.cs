using BenchmarkDotNet.Running;
using IPFees.Performance.Tests.Benchmarks;

namespace IPFees.Performance.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Run all benchmarks
            var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            
            // Alternative: Run specific benchmarks
            // BenchmarkRunner.Run<DslCalculatorBenchmarks>();
            // BenchmarkRunner.Run<FeeCalculatorBenchmarks>();
            // BenchmarkRunner.Run<JurisdictionFeeManagerBenchmarks>();
            // BenchmarkRunner.Run<EndToEndBenchmarks>();
        }
    }
}
