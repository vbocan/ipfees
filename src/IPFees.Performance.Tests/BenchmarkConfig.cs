using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

namespace IPFees.Performance.Tests
{
    /// <summary>
    /// Custom configuration for IPFees benchmarks
    /// Configures output formats, diagnostics, and job settings
    /// </summary>
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            // Add HTML, Markdown, CSV, and JSON exporters
            AddExporter(HtmlExporter.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddExporter(CsvExporter.Default);
            // Note: JsonExporter removed - not available in BenchmarkDotNet 0.14.0

            // Add console logger with color
            AddLogger(ConsoleLogger.Default);

            // Add memory diagnoser to track allocations
            AddDiagnoser(MemoryDiagnoser.Default);

            // Add columns for better readability
            AddColumn(StatisticColumn.Mean);
            AddColumn(StatisticColumn.StdDev);
            AddColumn(StatisticColumn.Median);
            AddColumn(StatisticColumn.P95);
            AddColumn(RankColumn.Arabic);

            // Configure job for accurate measurement
            AddJob(Job.Default
                .WithWarmupCount(3)      // Warmup iterations
                .WithIterationCount(10)   // Measurement iterations
                .WithLaunchCount(1)       // Number of process launches
                .WithId("IPFees-Benchmark"));
        }
    }

    /// <summary>
    /// Quick configuration for faster feedback during development
    /// </summary>
    public class QuickBenchmarkConfig : ManualConfig
    {
        public QuickBenchmarkConfig()
        {
            AddExporter(MarkdownExporter.Console);
            AddLogger(ConsoleLogger.Default);
            AddDiagnoser(MemoryDiagnoser.Default);

            AddJob(Job.Default
                .WithWarmupCount(1)
                .WithIterationCount(3)
                .WithLaunchCount(1)
                .WithId("IPFees-Quick"));
        }
    }
}
