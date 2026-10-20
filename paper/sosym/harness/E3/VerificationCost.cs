using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Harness.Common;
using IPFees.Core.FeeCalculation;
using IPFLang.Analysis;
using N = IPFLang.Parser;
using NE = IPFLang.Engine;

namespace Harness.E3;

/// <summary>
/// E3: what verification costs at authoring time. For every composed schedule, the production
/// save gate (FeeDefinitionValidator) and the ScheduleVerifier it delegates to, both under the
/// 5 s budget; then every VERIFY directive on its own through ScheduleVerifier. Each measurement
/// runs in a fresh process, alongside the size of the input space the checker works over.
/// </summary>
public static class VerificationCost
{
    private static readonly TimeSpan Budget = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan Kill = Budget + TimeSpan.FromSeconds(30);
    private const int Repetitions = 5;

    public static int Run(string[] args)
    {
        var outDir = Paths.ResultsFor("e3");
        var codes = args.Length > 0 ? args.ToList() : PackagedCorpus.Codes.ToList();
        var clock = Stopwatch.StartNew();
        WriteEnvironment(outDir);

        using var gate = new Csv(Path.Combine(outDir, "schedules.csv"),
            "code", "base", "inputs", "log10_input_space", "fees", "verify_complete", "verify_monotonic",
            "gate_verdict", "may_be_saved", "unfinished_directives",
            "gate_ms_first", "gate_ms_median", "gate_ms_max", "verifier_ms_median", "verifier_ms_max",
            "completeness_failures", "monotonicity_failures", "directive_errors");
        using var directives = new Csv(Path.Combine(outDir, "directives.csv"),
            "code", "index", "kind", "fee", "with_respect_to", "variables", "log10_space", "method_expected",
            "log10_contexts", "points_per_context", "log10_planned_evaluations",
            "verdict", "unfinished", "ms", "method", "combinations_checked", "points_checked",
            // Baseline: the engine's own entry point (DslCalculator.RunVerifications), which hands
            // the monotonicity checker every declared input of the composed schedule.
            "engine_variables", "engine_log10_contexts", "engine_log10_planned_evaluations",
            "engine_verdict", "engine_unfinished", "engine_ms");

        foreach (var code in codes)
        {
            var (script, _) = PackagedCorpus.Compose(code);
            if (script is null) { Console.Error.WriteLine($"{code}: does not compose"); continue; }
            var domains = new DomainAnalyzer().ExtractDomains(script.Inputs).ToList();
            var verifications = script.Verifications.ToList();

            var g = Worker.Run(Kill, "e3-worker", code, "gate");
            gate.Row(code, PackagedCorpus.BaseName(code), domains.Count, Math.Round(DomainStats.Log10InputSpace(domains), 2), script.Fees.Count(),
                verifications.Count(v => v is N.DslVerifyComplete), verifications.Count(v => v is N.DslVerifyMonotonic),
                Str(g, "verdict") ?? "killed", Bool(g, "mayBeSaved"), Num(g, "unfinished") ?? -1,
                Num(g, "msFirst"), Num(g, "msMedian"), Num(g, "msMax"), Num(g, "verifierMedian"), Num(g, "verifierMax"),
                Num(g, "completenessFailures"), Num(g, "monotonicityFailures"), Num(g, "directiveErrors"));

            for (int i = 0; i < verifications.Count; i++)
            {
                var v = verifications[i];
                var fee = script.Fees.LastOrDefault(f => f.Name == v.FeeName);
                object? vars = null, space = null, method = null, contexts = null, points = null, planned = null;
                object? engineVars = null, engineContexts = null, enginePlanned = null;
                JsonElement? engine = null;
                if (fee is not null && v is N.DslVerifyComplete)
                {
                    var c = DomainStats.Completeness(fee, domains);
                    (vars, space, method) = (c.Variables, Math.Round(c.Log10Size, 2), c.Method);
                }
                if (fee is not null && v is N.DslVerifyMonotonic vm)
                {
                    // ScheduleVerifier hands the checker the target and the inputs the fee reads.
                    var read = ScheduleVerifier.InputsReadBy(fee);
                    var relevant = domains.Where(d => d.VariableName == vm.WithRespectTo || read.Contains(d.VariableName)).ToList();
                    var m = DomainStats.Monotonicity(vm.WithRespectTo, relevant);
                    (vars, contexts, points, planned) = (relevant.Count - 1, Math.Round(m.Log10Contexts, 2), m.Points, Math.Round(m.Log10Evaluations, 2));

                    var all = DomainStats.Monotonicity(vm.WithRespectTo, domains);
                    (engineVars, engineContexts, enginePlanned) = (domains.Count - 1, Math.Round(all.Log10Contexts, 2), Math.Round(all.Log10Evaluations, 2));
                    engine = Worker.Run(Kill, "e3-worker", code, "engine", i.ToString());
                }
                var d = Worker.Run(Kill, "e3-worker", code, "directive", i.ToString());
                directives.Row(code, i, v is N.DslVerifyComplete ? "COMPLETE" : "MONOTONIC", v.FeeName, (v as N.DslVerifyMonotonic)?.WithRespectTo,
                    vars, space, method, contexts, points, planned,
                    Str(d, "verdict") ?? "killed", Bool(d, "unfinished") ?? true, Num(d, "ms"), Str(d, "method"), Num(d, "combinations"), Num(d, "points"),
                    engineVars, engineContexts, enginePlanned,
                    v is N.DslVerifyMonotonic ? Str(engine, "verdict") ?? "killed" : null,
                    v is N.DslVerifyMonotonic ? Bool(engine, "unfinished") ?? true : null,
                    v is N.DslVerifyMonotonic ? Num(engine, "ms") : null);
            }
            Console.WriteLine($"{code}: gate {Str(g, "verdict") ?? "killed"} in {Num(g, "msFirst")} ms, {verifications.Count} directives ({clock.Elapsed.TotalMinutes:N1} min)");
        }
        return 0;
    }

    /// <summary>Child process: one measurement, printed as JSON.</summary>
    public static int Work(string[] args)
    {
        var code = args[0];
        var budget = new VerificationBudget { Limit = Budget };
        if (args[1] == "gate")
        {
            var source = PackagedCorpus.Source(code);
            var refs = PackagedCorpus.ReferencedModules(code);
            var validator = PackagedCorpus.Validator(Budget);
            var times = new List<double>();
            var sw = Stopwatch.StartNew();
            var report = validator.ValidateAsync(source, refs).GetAwaiter().GetResult();
            times.Add(sw.Elapsed.TotalMilliseconds);

            var verifier = new List<double>();
            if (!report.VerificationTimedOut)
            {
                for (int i = 0; i < Repetitions; i++)
                {
                    sw.Restart();
                    validator.ValidateAsync(source, refs).GetAwaiter().GetResult();
                    times.Add(sw.Elapsed.TotalMilliseconds);
                }
                // The verifier alone, on the composed script the validator builds.
                var (script, _) = PackagedCorpus.Compose(code);
                for (int i = 0; i < Repetitions; i++)
                {
                    sw.Restart();
                    new ScheduleVerifier(budget).Verify(script!.Inputs, script.Fees, script.Verifications);
                    verifier.Add(sw.Elapsed.TotalMilliseconds);
                }
            }
            var warm = times.Count > 1 ? times.Skip(1).OrderBy(t => t).ToList() : times;
            verifier.Sort();
            return Worker.Finish(new
            {
                verdict = Gate.Verdict(report),
                mayBeSaved = report.MayBeSaved,
                unfinished = report.UnfinishedDirectives.Count,
                msFirst = Math.Round(times[0], 1),
                msMedian = Math.Round(warm[warm.Count / 2], 2),
                msMax = Math.Round(warm.Max(), 2),
                verifierMedian = verifier.Count > 0 ? Math.Round(verifier[verifier.Count / 2], 2) : (double?)null,
                verifierMax = verifier.Count > 0 ? Math.Round(verifier.Max(), 2) : (double?)null,
                completenessFailures = report.CompletenessFailures.Count,
                monotonicityFailures = report.MonotonicityFailures.Count,
                directiveErrors = report.ParseErrors.Count
            });
        }
        else if (args[1] == "engine")
        {
            // The engine's own entry point on this directive alone: RunVerifications passes the
            // monotonicity checker every input of the composed schedule. The engine takes no
            // cancellation, so an overrun is abandoned at the budget and the process exits.
            var index = int.Parse(args[2]);
            var (script, _) = PackagedCorpus.Compose(code);
            var directive = script!.Verifications.ElementAt(index);
            var single = new IPFLang.Versioning.ParsedScript(script.Inputs, script.Fees, script.Returns, script.Groups, new[] { directive });
            var calculator = new NE.DslCalculator(new N.DslParser());
            calculator.LoadParsedScript(single);
            var sw = Stopwatch.StartNew();
            var running = Task.Run(() => calculator.RunVerifications());
            if (!running.Wait(Budget))
                return Worker.Finish(new { verdict = "unfinished", unfinished = true, ms = Math.Round(sw.Elapsed.TotalMilliseconds, 1) });
            var m = running.Result.MonotonicityReports.FirstOrDefault();
            return Worker.Finish(new
            {
                verdict = m is null ? "no report" : m.IsMonotonic ? "monotonic" : $"not monotonic ({m.Violations.Count} violations)",
                unfinished = false,
                ms = Math.Round(sw.Elapsed.TotalMilliseconds, 2)
            });
        }
        else
        {
            var index = int.Parse(args[2]);
            var (script, _) = PackagedCorpus.Compose(code);
            var directive = script!.Verifications.ElementAt(index);
            var sw = Stopwatch.StartNew();
            var r = new ScheduleVerifier(budget).Verify(script.Inputs, script.Fees, new[] { directive });
            var ms = sw.Elapsed.TotalMilliseconds;
            var c = r.Completeness.FirstOrDefault();
            var m = r.Monotonicity.FirstOrDefault();
            var verdict = r.Unfinished.Count > 0 ? "unfinished"
                : r.Errors.Count > 0 ? "error: " + string.Join("; ", r.Errors)
                : c is not null ? (c.IsComplete ? "complete" : $"incomplete ({c.Gaps.Count} gaps)")
                : m is not null ? (m.IsMonotonic ? "monotonic" : $"not monotonic ({m.Violations.Count} violations)")
                : "no report";
            return Worker.Finish(new
            {
                verdict,
                unfinished = r.Unfinished.Count > 0,
                ms = Math.Round(ms, 2),
                method = c?.VerificationMethod,
                combinations = c?.TotalCombinationsChecked,
                points = m?.PointsChecked
            });
        }
    }

    private static void WriteEnvironment(string outDir)
    {
        var info = new
        {
            measured = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            os = RuntimeInformation.OSDescription,
            framework = RuntimeInformation.FrameworkDescription,
            architecture = RuntimeInformation.ProcessArchitecture.ToString(),
            logicalProcessors = Environment.ProcessorCount,
            cpu = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
            engine = typeof(NE.DslCalculator).Assembly.GetName().Version?.ToString(),
            platform = File.Exists(Path.Combine(Paths.Work, "revisions.txt")) ? File.ReadAllText(Path.Combine(Paths.Work, "revisions.txt")).Trim() : null,
            budgetSeconds = Budget.TotalSeconds,
            repetitions = Repetitions,
            note = "each measurement runs in a fresh process, sequentially; the gate's first run includes JIT"
        };
        File.WriteAllText(Path.Combine(outDir, "environment.json"), JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static string? Str(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    private static bool? Bool(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind is JsonValueKind.True or JsonValueKind.False ? v.GetBoolean() : null;
    private static double? Num(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : null;
}

/// <summary>The decision the fee editor makes on a validation report (Fee/Create and Fee/Edit: refuse unless MayBeSaved).</summary>
public static class Gate
{
    public static bool Blocks(FeeValidationReport r) => !r.MayBeSaved;

    /// <summary>Type errors raised by the validator's own currency resolution rather than the engine's type checker.</summary>
    public static bool IsCurrencyResolution(string typeError) => typeError.StartsWith("[MixedFeeCurrencies]", StringComparison.Ordinal);

    public static string Verdict(FeeValidationReport r) =>
        r.ParseErrors.Count > 0 ? "blocked: parse or directive error"
        : r.TypeErrors.Any(e => !IsCurrencyResolution(e)) ? "blocked: type error"
        : r.TypeErrors.Count > 0 ? "blocked: fee currency unresolvable"
        : r.CompletenessFailures.Count > 0 && r.MonotonicityFailures.Count > 0 ? "blocked: incomplete and not monotonic"
        : r.CompletenessFailures.Count > 0 ? "blocked: incomplete"
        : r.MonotonicityFailures.Count > 0 ? "blocked: not monotonic"
        : r.VerificationTimedOut ? "saved: some directives unfinished"
        : r.HasNoVerifications ? "saved: no directives"
        : "saved: verified";
}
