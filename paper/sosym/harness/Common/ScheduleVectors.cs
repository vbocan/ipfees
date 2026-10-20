using IPFLang.Versioning;
using N = IPFLang.Parser;

namespace Harness.Common;

/// <summary>
/// Input vectors for one composed schedule: list choices, booleans both ways, numeric
/// defaults and bounds, guard literals ±1, capped by pairwise combination. Used by E2 as the
/// oracle that tells whether a seeded fault changes any computed amount.
/// </summary>
public sealed class ScheduleVectors
{
    public IReadOnlyList<Parameter> Parameters { get; }
    public IReadOnlyList<int[]> Rows { get; }
    public IReadOnlyList<List<IPFLang.Evaluator.IPFValue>> Values { get; }

    public ScheduleVectors(ParsedScript script, DateOnly today)
    {
        var inputs = new Dictionary<string, InputDecl>();
        foreach (var i in script.Inputs) inputs.TryAdd(i.Name, InputDecl.From(i));

        var guards = new Guards();
        foreach (var fee in script.Fees)
        {
            var lets = fee.Vars.ToDictionary(v => v.Name.Contains('.') ? v.Name[(v.Name.IndexOf('.') + 1)..] : v.Name, v => (IReadOnlyList<string>)v.ValueTokens.ToList());
            foreach (var item in fee.Cases)
            {
                if (item is N.DslFeeCase c)
                {
                    guards.Add(c.Condition.ToList(), lets, inputs);
                    foreach (var y in c.Yields) guards.Add(y.Condition.ToList(), lets, inputs);
                }
                else if (item is N.DslFeeYield y)
                {
                    guards.Add(y.Condition.ToList(), lets, inputs);
                }
            }
        }

        Parameters = inputs.Keys.OrderBy(n => n, StringComparer.Ordinal).Select(n => InputSpace.Build(inputs[n], guards, today)).ToList();
        var rows = Pairwise.Generate(Parameters.Select(p => p.Values.Count).ToList(), Parameters.Select(p => p.Preferred).ToList());
        rows.Insert(0, Parameters.Select(p => p.Preferred).ToArray());
        Rows = rows;
        Values = rows.Select(r => Parameters.Select((p, i) => Common.Values.ToEngine(p.Name, p.Values[r[i]])).ToList()).ToList();
    }

    /// <summary>The same rows with one input swept across all its values, for the monotonicity oracle.</summary>
    public IEnumerable<List<IPFLang.Evaluator.IPFValue>> Sweep(int row, string input)
    {
        var index = Parameters.ToList().FindIndex(p => p.Name == input);
        if (index < 0) yield break;
        var p = Parameters[index];
        foreach (var v in p.Values.OrderBy(v => v is decimal d ? d : 0m))
        {
            var values = new List<IPFLang.Evaluator.IPFValue>(Values[row]);
            values[index] = Common.Values.ToEngine(p.Name, v);
            yield return values;
        }
    }
}
