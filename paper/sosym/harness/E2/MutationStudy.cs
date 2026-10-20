using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Harness.Common;
using Harness.E3;
using IPFees.Core.FeeCalculation;
using IPFLang.Versioning;
using N = IPFLang.Parser;
using NE = IPFLang.Engine;

namespace Harness.E2;

/// <summary>
/// E2: what the save gate catches. Every mutant goes through the production validator
/// (FeeDefinitionValidator, which runs ScheduleVerifier under the 5 s budget) and the fee pages'
/// own decision rule (refuse unless MayBeSaved). A mutant is detected when the gate refuses it
/// with a finding the unmutated schedule does not have: a parse or type error, or a directive
/// that fails on the mutant and holds on the original. Alongside runs an execution oracle on
/// pairwise input vectors that says whether the fault changes any amount, breaks a declared
/// monotonicity, or changes the currency IPFees converts the fee from.
/// </summary>
public static class MutationStudy
{
    private static readonly TimeSpan Budget = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan Kill = TimeSpan.FromMinutes(5);
    private const int TimedOutExit = 3;

    private static List<Mutant> AllMutants(IEnumerable<string>? codes = null) =>
        (codes ?? PackagedCorpus.Codes).SelectMany(c => Mutator.Enumerate(c, PackagedCorpus.Source(c))).ToList();

    public static int Run(string[] args)
    {
        var workers = args.FirstOrDefault(a => a.StartsWith("--workers="))?.Split('=')[1] is { } w ? int.Parse(w) : 8;
        var codes = args.Where(a => !a.StartsWith("--")).ToList();
        var selected = codes.Count > 0 ? codes : PackagedCorpus.Codes.ToList();
        var mutants = AllMutants();
        var indices = Enumerable.Range(0, mutants.Count).Where(i => selected.Contains(mutants[i].Code)).ToList();
        var outDir = Paths.ResultsFor("e2");
        var clock = Stopwatch.StartNew();
        Console.WriteLine($"E2: {indices.Count} mutants of {selected.Count} schedules, {workers} workers");

        // Baselines: the unmutated schedule through the same gate.
        var baselines = new ConcurrentDictionary<string, JsonElement?>();
        Parallel.ForEach(selected, new ParallelOptions { MaxDegreeOfParallelism = workers }, code =>
            baselines[code] = Worker.Run(Kill, "e2-baseline", code));
        Console.WriteLine($"baselines done in {clock.Elapsed.TotalMinutes:N1} min");

        // Mutants, sharded round-robin. A run abandoned at the budget leaves a verifier thread
        // spinning, so a worker exits after one and is restarted at the next mutant.
        var results = new ConcurrentDictionary<int, JsonElement>();
        var shards = Enumerable.Range(0, workers).Select(k => indices.Where((_, j) => j % workers == k).ToList()).ToList();
        Parallel.ForEach(shards, new ParallelOptions { MaxDegreeOfParallelism = workers }, shard =>
        {
            int next = 0;
            while (next < shard.Count)
            {
                var batch = string.Join(",", shard.Skip(next));
                var lines = Stream(Kill, "e2-worker", batch);
                foreach (var e in lines) results[e.GetProperty("index").GetInt32()] = e;
                var done = shard.FindLastIndex(i => results.ContainsKey(i));
                if (done < next)
                {
                    // Nothing came back: record the mutant as killed by the harness and move on.
                    results[shard[next]] = JsonDocument.Parse($"{{\"index\":{shard[next]},\"gate\":\"harness: worker killed\"}}").RootElement.Clone();
                    done = next;
                }
                next = done + 1;
                Console.WriteLine($"{results.Count}/{indices.Count} ({clock.Elapsed.TotalMinutes:N1} min)");
            }
        });
        Console.WriteLine($"E2 done in {clock.Elapsed.TotalMinutes:N1} min");

        Write(outDir, mutants, indices, results, baselines);
        return 0;
    }

    /// <summary>Child process: the unmutated schedule.</summary>
    public static int Baseline(string[] args)
    {
        var code = args[0];
        var report = PackagedCorpus.Validator(Budget).ValidateAsync(PackagedCorpus.Source(code), PackagedCorpus.ReferencedModules(code)).GetAwaiter().GetResult();
        return Worker.Finish(new
        {
            code,
            gate = Gate.Verdict(report),
            blocked = Gate.Blocks(report),
            unfinished = report.UnfinishedDirectives,
            incomplete = report.CompletenessFailures.Select(FeeNamed).ToList(),
            notMonotonic = report.MonotonicityFailures.Select(FeeNamed).ToList()
        });
    }

    /// <summary>Child process: a list of mutant indices, one JSON line each.</summary>
    public static int Work(string[] args)
    {
        var indices = args[0].Split(',').Select(int.Parse).ToList();
        var mutants = AllMutants();
        var today = DateOnly.FromDateTime(DateTime.Now);
        var originals = new Dictionary<string, (ParsedScript Script, ScheduleVectors Vectors)>();

        foreach (var index in indices)
        {
            try { WorkOne(index, mutants, originals, today); }
            catch (Exception ex)
            {
                Console.Out.WriteLine(JsonSerializer.Serialize(new { index, gate = "harness: " + ex.GetType().Name + ": " + ex.Message }));
                Console.Out.Flush();
            }
        }
        Environment.Exit(0);
        return 0;
    }

    private static void WorkOne(int index, List<Mutant> mutants, Dictionary<string, (ParsedScript Script, ScheduleVectors Vectors)> originals, DateOnly today)
    {
        var m = mutants[index];
        if (!originals.TryGetValue(m.Code, out var original))
        {
            var (s, _) = PackagedCorpus.Compose(m.Code);
            originals[m.Code] = original = (s!, new ScheduleVectors(s!, today));
        }

        var sw = Stopwatch.StartNew();
        var report = PackagedCorpus.Validator(Budget).ValidateAsync(m.Source, PackagedCorpus.ReferencedModules(m.Code)).GetAwaiter().GetResult();
        var ms = sw.Elapsed.TotalMilliseconds;

        var (mutated, composeErrors) = PackagedCorpus.Compose(m.Code, m.Source);
        int? changed = null;
        bool? monotonicViolated = null, originalMonotonicViolated = null;
        string? currencyBefore = null, currencyAfter = null, oracleNote = null;

        if (mutated is not null)
        {
            var originalFee = original.Script.Fees.LastOrDefault(f => f.Name == m.Fee);
            var mutatedFee = mutated.Fees.LastOrDefault(f => f.Name == m.Fee);
            if (originalFee is not null && mutatedFee is not null)
            {
                var oc = FeeEval.CalculatorFor(original.Script, originalFee);
                var mc = FeeEval.CalculatorFor(mutated, mutatedFee);
                changed = 0;
                foreach (var v in original.Vectors.Values)
                {
                    var (oa, oe) = FeeEval.Fee(oc, v);
                    var (ma, me) = FeeEval.Fee(mc, v);
                    if (oa != ma || (oe is null) != (me is null)) changed++;
                }

                foreach (var directive in original.Script.Verifications.OfType<N.DslVerifyMonotonic>().Where(d => d.FeeName == m.Fee))
                {
                    var direction = Enum.TryParse<IPFLang.Analysis.MonotonicityDirection>(directive.Direction, out var d) ? d : IPFLang.Analysis.MonotonicityDirection.NonDecreasing;
                    monotonicViolated = (monotonicViolated ?? false) | Violates(mc, original.Vectors, directive.WithRespectTo, direction);
                    originalMonotonicViolated = (originalMonotonicViolated ?? false) | Violates(oc, original.Vectors, directive.WithRespectTo, direction);
                }

                // The currency FeeCalculator converts this fee from (FeeCurrency.Resolve); a fee
                // whose yields resolve to two currencies makes the calculation fail.
                currencyBefore = ResolveCurrency(originalFee, original.Script);
                currencyAfter = ResolveCurrency(mutatedFee, mutated);
            }
            else oracleNote = "fee not found after mutation";
        }
        else oracleNote = "mutant does not compose: " + string.Join("; ", composeErrors.Take(2));

        var hasComplete = original.Script.Verifications.OfType<N.DslVerifyComplete>().Any(v => v.FeeName == m.Fee);
        var hasMonotonic = original.Script.Verifications.OfType<N.DslVerifyMonotonic>().Any(v => v.FeeName == m.Fee);

        Console.Out.WriteLine(JsonSerializer.Serialize(new
        {
            index,
            gate = Gate.Verdict(report),
            blocked = Gate.Blocks(report),
            unfinished = report.UnfinishedDirectives,
            ms = Math.Round(ms, 1),
            firstMessage = report.ParseErrors.Concat(report.TypeErrors).Concat(report.CompletenessFailures).Concat(report.MonotonicityFailures).FirstOrDefault(),
            engineTypeErrors = report.TypeErrors.Count(e => !Gate.IsCurrencyResolution(e)),
            currencyResolutionErrors = report.TypeErrors.Count(Gate.IsCurrencyResolution),
            incomplete = report.CompletenessFailures.Select(FeeNamed).ToList(),
            notMonotonic = report.MonotonicityFailures.Select(FeeNamed).ToList(),
            hasComplete,
            hasMonotonic,
            vectors = original.Vectors.Rows.Count,
            changed,
            monotonicViolated,
            originalMonotonicViolated,
            currencyBefore,
            currencyAfter,
            oracleNote
        }));
        Console.Out.Flush();
        if (report.VerificationTimedOut) Environment.Exit(TimedOutExit);
    }

    private static string ResolveCurrency(N.DslFee fee, ParsedScript script)
    {
        try { return FeeCurrency.Resolve(fee, script.Inputs, FeeEval.Currency(script) ?? ""); }
        catch (InvalidOperationException) { return "unresolvable"; }
    }

    /// <summary>VerificationNarrative opens every failure with the fee name in quotes.</summary>
    private static string FeeNamed(string narrative)
    {
        var a = narrative.IndexOf('\'');
        var b = a < 0 ? -1 : narrative.IndexOf('\'', a + 1);
        return a >= 0 && b > a ? narrative[(a + 1)..b] : narrative;
    }

    /// <summary>
    /// Execution oracle for monotonicity: sweep the input across every value the vector
    /// generator gives it (bounds, defaults, each guard literal ±1) in every pairwise context,
    /// and look for a step in the wrong direction.
    /// </summary>
    private static bool Violates(NE.DslCalculator calculator, ScheduleVectors vectors, string input, IPFLang.Analysis.MonotonicityDirection direction)
    {
        for (int row = 0; row < vectors.Rows.Count; row++)
        {
            decimal? previous = null;
            foreach (var values in vectors.Sweep(row, input))
            {
                var (amount, error) = FeeEval.Fee(calculator, values);
                if (error is not null || amount is null) continue;
                if (previous is { } p)
                {
                    var bad = direction switch
                    {
                        IPFLang.Analysis.MonotonicityDirection.NonDecreasing => amount < p,
                        IPFLang.Analysis.MonotonicityDirection.NonIncreasing => amount > p,
                        IPFLang.Analysis.MonotonicityDirection.StrictlyIncreasing => amount <= p,
                        IPFLang.Analysis.MonotonicityDirection.StrictlyDecreasing => amount >= p,
                        _ => false
                    };
                    if (bad) return true;
                }
                previous = amount;
            }
        }
        return false;
    }

    private static List<JsonElement> Stream(TimeSpan kill, params string[] args)
    {
        var self = typeof(Worker).Assembly.Location;
        var host = Environment.ProcessPath!;
        var psi = new ProcessStartInfo(host) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        if (Path.GetFileNameWithoutExtension(host).Equals("dotnet", StringComparison.OrdinalIgnoreCase)) psi.ArgumentList.Add(self);
        foreach (var a in args) psi.ArgumentList.Add(a);

        var lines = new List<JsonElement>();
        using var process = Process.Start(psi)!;
        var stderr = process.StandardError.ReadToEndAsync();
        var reader = Task.Run(() =>
        {
            string? line;
            while ((line = process.StandardOutput.ReadLine()) is not null)
                if (line.StartsWith("{")) lock (lines) lines.Add(JsonDocument.Parse(line).RootElement.Clone());
        });
        // A worker that hangs outside the budgeted call (it should not) is killed; the
        // mutant it was on is then recorded as killed by the harness.
        var watchdog = Stopwatch.StartNew();
        int seen = 0;
        while (!process.WaitForExit(1000))
        {
            lock (lines) if (lines.Count != seen) { seen = lines.Count; watchdog.Restart(); }
            if (watchdog.Elapsed > kill) { try { process.Kill(true); } catch { } break; }
        }
        reader.Wait(TimeSpan.FromSeconds(10));
        return lines;
    }

    private static void Write(string outDir, List<Mutant> mutants, List<int> indices, ConcurrentDictionary<int, JsonElement> results, ConcurrentDictionary<string, JsonElement?> baselines)
    {
        using (var csv = new Csv(Path.Combine(outDir, "baselines.csv"), "code", "gate", "may_be_saved", "unfinished", "incomplete", "not_monotonic"))
        {
            foreach (var (code, b) in baselines.OrderBy(kv => kv.Key, StringComparer.Ordinal))
                csv.Row(code, S(b, "gate") ?? "harness: killed", B(b, "blocked") is { } x ? !x : null,
                    string.Join(" | ", L(b, "unfinished")), string.Join(" ", L(b, "incomplete")), string.Join(" ", L(b, "notMonotonic")));
        }

        var rows = new List<Row>();
        using (var csv = new Csv(Path.Combine(outDir, "mutants.csv"),
            "id", "code", "operator", "fee", "line", "original", "mutated", "detail",
            "fee_has_verify_complete", "fee_has_verify_monotonic", "original_refused", "gate", "refused", "unfinished_directives",
            "detected", "detected_by", "ms", "gate_incomplete", "gate_not_monotonic", "gate_message",
            "vectors", "vectors_changed", "observable", "monotonic_oracle_violated", "currency_original", "currency_mutant", "currency_resolution_fired", "oracle_note"))
        {
            foreach (var i in indices)
            {
                var m = mutants[i];
                var r = results.TryGetValue(i, out var e) ? e : (JsonElement?)null;
                var b = baselines.GetValueOrDefault(m.Code);
                var originalRefused = B(b, "blocked") ?? false;
                var refused = B(r, "blocked");
                var gate = S(r, "gate") ?? "";
                var newIncomplete = L(r, "incomplete").Except(L(b, "incomplete")).ToList();
                var newNotMonotonic = L(r, "notMonotonic").Except(L(b, "notMonotonic")).ToList();
                // What the gate found on the mutant that it does not find on the original.
                var engineType = N(r, "engineTypeErrors") ?? 0;
                var currencyResolution = N(r, "currencyResolutionErrors") ?? 0;
                // The engine's type checker is credited whenever it fires; currency resolution
                // only when it is the sole reason for the refusal.
                var by = refused != true ? ""
                    : gate.Contains("parse") ? "parse or directive error"
                    : engineType > 0 ? "type error"
                    : currencyResolution > 0 ? "currency resolution"
                    : newNotMonotonic.Count > 0 && newIncomplete.Count > 0 ? "new completeness and monotonicity failures"
                    : newNotMonotonic.Count > 0 ? "new monotonicity failure"
                    : newIncomplete.Count > 0 ? "new completeness failure"
                    : "";
                var detected = by != "";
                var changed = N(r, "changed");
                bool? observable = changed is null ? null : changed > 0;
                bool? monotonicNew = B(r, "monotonicViolated") is { } mv ? mv && B(r, "originalMonotonicViolated") != true : null;
                var unfinished = L(r, "unfinished").Count;
                var row = new Row(m, gate, refused, unfinished, detected, by, originalRefused,
                    B(r, "hasComplete"), B(r, "hasMonotonic"), observable, monotonicNew, S(r, "currencyBefore"), S(r, "currencyAfter"), currencyResolution > 0);
                rows.Add(row);
                csv.Row(m.Id, m.Code, m.Operator, m.Fee, m.Line, m.Original, m.Mutated, m.Detail,
                    row.HasComplete, row.HasMonotonic, originalRefused, gate, refused, unfinished,
                    detected, by, N(r, "ms"), string.Join(" ", L(r, "incomplete")), string.Join(" ", L(r, "notMonotonic")), S(r, "firstMessage"),
                    N(r, "vectors"), changed, observable, monotonicNew, row.CurrencyBefore, row.CurrencyAfter, row.CurrencyResolutionFired, S(r, "oracleNote"));
            }
        }

        using (var csv = new Csv(Path.Combine(outDir, "detection.csv"),
            "operator", "mutants", "schedules", "detected", "detection_rate",
            "by_type", "by_currency_resolution", "currency_resolution_fired", "by_parse", "by_completeness", "by_monotonicity",
            "refused_without_new_finding", "saved", "saved_fee_has_no_directive", "with_unfinished_directive",
            "original_saved", "detected_where_original_saved", "rate_where_original_saved",
            "observable", "detected_of_observable", "rate_of_observable",
            "monotonic_oracle_violations", "monotonic_violations_detected",
            "currency_changed", "currency_unresolvable", "currency_unresolvable_saved"))
        {
            foreach (var op in Mutator.Operators.Concat(new[] { "ALL" }))
            {
                var all = rows.Where(r => op == "ALL" || r.M.Operator == op).ToList();
                var detected = all.Count(r => r.Detected);
                var originalSaved = all.Where(r => !r.OriginalRefused).ToList();
                var observable = all.Where(r => r.Observable == true).ToList();
                var mono = all.Where(r => r.MonotonicNew == true).ToList();
                var currencyChanged = all.Where(r => r.CurrencyBefore is not null && r.CurrencyAfter is not null && r.CurrencyBefore != r.CurrencyAfter).ToList();
                var unresolvable = currencyChanged.Where(r => r.CurrencyAfter == "unresolvable").ToList();
                csv.Row(op, all.Count, all.Select(r => r.M.Code).Distinct().Count(), detected, Rate(detected, all.Count),
                    all.Count(r => r.By == "type error"), all.Count(r => r.By == "currency resolution"), all.Count(r => r.CurrencyResolutionFired),
                    all.Count(r => r.By == "parse or directive error"),
                    all.Count(r => r.By.Contains("completeness")), all.Count(r => r.By.Contains("monotonicity")),
                    all.Count(r => r.Refused == true && !r.Detected), all.Count(r => r.Refused == false),
                    all.Count(r => r.Refused == false && r.HasComplete == false && r.HasMonotonic == false),
                    all.Count(r => r.Unfinished > 0),
                    originalSaved.Count, originalSaved.Count(r => r.Detected), Rate(originalSaved.Count(r => r.Detected), originalSaved.Count),
                    observable.Count, observable.Count(r => r.Detected), Rate(observable.Count(r => r.Detected), observable.Count),
                    mono.Count, mono.Count(r => r.Detected),
                    currencyChanged.Count, unresolvable.Count, unresolvable.Count(r => r.Refused == false));
            }
        }
    }

    private sealed record Row(Mutant M, string Gate, bool? Refused, int Unfinished, bool Detected, string By, bool OriginalRefused,
        bool? HasComplete, bool? HasMonotonic, bool? Observable, bool? MonotonicNew, string? CurrencyBefore, string? CurrencyAfter, bool CurrencyResolutionFired);

    private static string Rate(int a, int b) => b == 0 ? "" : (100.0 * a / b).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
    private static List<string> L(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray().Select(i => i.GetString() ?? "").ToList() : new List<string>();
    private static string? S(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    private static bool? B(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind is JsonValueKind.True or JsonValueKind.False ? v.GetBoolean() : null;
    private static double? N(JsonElement? e, string p) => e is { } x && x.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : null;
}
