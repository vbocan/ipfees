using System.Globalization;
using System.Text.RegularExpressions;

namespace Harness.E2;

public sealed record Mutant(
    string Id, string Code, string Operator, string Fee, int Line,
    string Original, string Mutated, string Detail, string Source);

/// <summary>
/// Seeded faults for E2, applied to the text of a jurisdiction script (the leaf the author
/// edits and the validator receives; the regional bases hold no fee definitions). Every
/// operator rewrites one site, and the sites of a script are enumerated in source order so the
/// mutant set is the same on every run.
/// </summary>
public static class Mutator
{
    public static readonly string[] Operators =
    {
        "DEL_CASE",      // delete a CASE ... ENDCASE branch
        "DEL_YIELD_IF",  // delete a conditional YIELD (a branch written without CASE)
        "NARROW_GUARD",  // tighten a numeric guard by one unit
        "CURRENCY",      // change the currency of an amount literal
        "MIX_CURRENCY",  // make one YIELD add two currencies
        "INVERT_MONO",   // negate a comparison inside a fee that has VERIFY MONOTONIC
        "CHANGE_CONST",  // change a numeric constant in a LET or YIELD value
    };

    private static readonly Regex Compute = new(@"^\s*COMPUTE\s+FEE\s+(\w+)", RegexOptions.Compiled);
    private static readonly Regex Case = new(@"^\s*CASE\s+(.*)\s+AS\s*(#.*)?$", RegexOptions.Compiled);
    private static readonly Regex Yield = new(@"^(\s*YIELD\s+)(.*)$", RegexOptions.Compiled);
    private static readonly Regex Let = new(@"^(\s*LET\s+\w+\s+AS\s+)(.*)$", RegexOptions.Compiled);
    private static readonly Regex Monotonic = new(@"^\s*VERIFY\s+MONOTONIC\s+FEE\s+(\w+)\s+WITH\s+RESPECT\s+TO\s+(\w+)", RegexOptions.Compiled);
    private static readonly Regex Currency = new(@"^\s*RETURN\s+Currency\s+AS\s+'([A-Z]{3})'", RegexOptions.Compiled);
    private static readonly Regex Comparison = new(@"(?<lhs>[A-Za-z_][\w!]*)\s+(?<op>GTE|GT|LTE|LT|EQ|NEQ)\s+(?<rhs>-?\d+(?:\.\d+)?|[A-Za-z_]\w*)", RegexOptions.Compiled);
    private static readonly Regex AmountLiteral = new(@"(?<v>-?\d+(?:\.\d+)?)<(?<c>[A-Z]{3})>", RegexOptions.Compiled);
    private static readonly Regex Number = new(@"(?<![\w.<])(?<v>\d+(?:\.\d+)?)(?![\w.])", RegexOptions.Compiled);

    public static List<Mutant> Enumerate(string code, string source)
    {
        var newline = source.Contains("\r\n") ? "\r\n" : "\n";
        var lines = source.Replace("\r\n", "\n").Split('\n');
        var mutants = new List<Mutant>();
        var scheduleCurrency = lines.Select(l => Currency.Match(l)).FirstOrDefault(m => m.Success)?.Groups[1].Value ?? "EUR";
        var monotonicFees = lines.Select(l => Monotonic.Match(l)).Where(m => m.Success)
            .GroupBy(m => m.Groups[1].Value).ToDictionary(g => g.Key, g => g.Select(m => m.Groups[2].Value).ToList());

        string Join(IEnumerable<string> ls) => string.Join(newline, ls);
        string Replace(int line, string text) => Join(lines.Select((l, i) => i == line ? text : l));
        string Delete(int from, int to) => Join(lines.Where((_, i) => i < from || i > to));
        void Add(string op, string fee, int line, string original, string mutated, string detail, string src) =>
            mutants.Add(new Mutant($"{code}:{op}:{mutants.Count(m => m.Operator == op)}", code, op, fee, line + 1, original.Trim(), mutated.Trim(), detail, src));

        string? fee = null;
        int caseStart = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            var raw = lines[i];
            var (body, comment) = SplitComment(raw);
            var compute = Compute.Match(body);
            if (compute.Success) { fee = compute.Groups[1].Value; continue; }
            if (body.Trim() == "ENDCOMPUTE") { fee = null; continue; }
            if (fee is null) continue;

            var isCase = Case.Match(body);
            if (isCase.Success)
            {
                caseStart = i;
                Guards(i, isCase.Groups[1].Index, isCase.Groups[1].Value, raw, fee);
                continue;
            }
            if (body.Trim() == "ENDCASE" && caseStart >= 0)
            {
                Add("DEL_CASE", fee, caseStart, lines[caseStart], "(deleted through ENDCASE)", $"lines {caseStart + 1}-{i + 1}", Delete(caseStart, i));
                caseStart = -1;
                continue;
            }

            var y = Yield.Match(body);
            if (y.Success)
            {
                var rest = y.Groups[2].Value;
                var ifAt = Regex.Match(rest, @"\sIF\s");
                var valueText = ifAt.Success ? rest[..ifAt.Index] : rest;
                var valueStart = y.Groups[2].Index;
                if (ifAt.Success)
                {
                    Add("DEL_YIELD_IF", fee, i, raw, "(deleted)", "", Delete(i, i));
                    var condStart = valueStart + ifAt.Index + ifAt.Length;
                    Guards(i, condStart, body[condStart..], raw, fee);
                }
                Values(i, valueStart, valueText, raw, fee);

                // MIX_CURRENCY: one YIELD adding two different currencies.
                var c = AmountLiteral.Match(valueText) is { Success: true } lit ? lit.Groups["c"].Value : scheduleCurrency;
                var d = c == "EUR" ? "USD" : "EUR";
                var end = valueStart + valueText.TrimEnd().Length;
                var mixed = body[..end] + $" + 1<{c}> + 1<{d}>" + body[end..] + comment;
                Add("MIX_CURRENCY", fee, i, raw, mixed, $"+ 1<{c}> + 1<{d}>", Replace(i, mixed));
                continue;
            }

            var let = Let.Match(body);
            if (let.Success) Values(i, let.Groups[2].Index, let.Groups[2].Value, raw, fee);
        }
        return mutants;

        // Guards: NARROW_GUARD on numeric comparisons, INVERT_MONO in fees with VERIFY MONOTONIC.
        void Guards(int line, int offset, string condition, string raw, string feeName)
        {
            foreach (Match m in Comparison.Matches(condition))
            {
                var op = m.Groups["op"].Value;
                var rhs = m.Groups["rhs"].Value;
                var at = offset + m.Index;
                var numeric = decimal.TryParse(rhs, NumberStyles.Number, CultureInfo.InvariantCulture, out var n);

                if (numeric && op is "GT" or "GTE" or "LT" or "LTE")
                {
                    var (nop, nv) = op switch
                    {
                        "GT" => ("GT", n + 1),
                        "GTE" => ("GT", n),
                        "LT" => ("LT", n - 1),
                        _ => ("LT", n)
                    };
                    var replacement = $"{m.Groups["lhs"].Value} {nop} {nv.ToString(CultureInfo.InvariantCulture)}";
                    var text = raw[..at] + replacement + raw[(at + m.Length)..];
                    Add("NARROW_GUARD", feeName, line, m.Value, replacement, "", Replace(line, text));
                }

                if (monotonicFees.TryGetValue(feeName, out var targets))
                {
                    var negated = op switch { "GT" => "LTE", "GTE" => "LT", "LT" => "GTE", "LTE" => "GT", "EQ" => "NEQ", _ => "EQ" };
                    var replacement = $"{m.Groups["lhs"].Value} {negated} {rhs}";
                    var text = raw[..at] + replacement + raw[(at + m.Length)..];
                    var onTarget = targets.Contains(m.Groups["lhs"].Value);
                    Add("INVERT_MONO", feeName, line, m.Value, replacement, onTarget ? "on the monotonic input" : "on another input", Replace(line, text));
                }
            }
        }

        // Values: CURRENCY on amount literals, CHANGE_CONST on every numeric constant.
        void Values(int line, int offset, string value, string raw, string feeName)
        {
            foreach (Match m in AmountLiteral.Matches(value))
            {
                var c = m.Groups["c"].Value;
                var d = c == "EUR" ? "USD" : "EUR";
                var at = offset + m.Groups["c"].Index;
                var text = raw[..at] + d + raw[(at + 3)..];
                Add("CURRENCY", feeName, line, m.Value, $"{m.Groups["v"].Value}<{d}>", "", Replace(line, text));
            }
            foreach (Match m in Number.Matches(value))
            {
                var v = decimal.Parse(m.Groups["v"].Value, CultureInfo.InvariantCulture);
                var changed = (v + 1).ToString(CultureInfo.InvariantCulture);
                var at = offset + m.Index;
                var text = raw[..at] + changed + raw[(at + m.Length)..];
                Add("CHANGE_CONST", feeName, line, m.Value, changed, "", Replace(line, text));
            }
        }
    }

    private static (string Body, string Comment) SplitComment(string line)
    {
        bool quoted = false;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '\'') quoted = !quoted;
            if (line[i] == '#' && !quoted) return (line[..i], line[i..]);
        }
        return (line, "");
    }
}
