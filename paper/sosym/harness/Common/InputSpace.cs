using System.Globalization;

namespace Harness.Common;

/// <summary>
/// Literal boundaries that appear in CASE and YIELD IF guards, resolved to the input they
/// constrain. A guard on a LET variable derived from a date (e.g. RenMonth AS
/// ROUND(ApplicationDate!MONTHSTONOW)) resolves to that date in the matching unit.
/// </summary>
public sealed class Guards
{
    public enum DateUnit { Days, Months, Years }

    public Dictionary<string, HashSet<decimal>> Numeric { get; } = new();
    public Dictionary<string, HashSet<int>> Count { get; } = new();
    public Dictionary<string, HashSet<(DateUnit Unit, decimal Value)>> Date { get; } = new();
    public HashSet<string> Unresolved { get; } = new();
    public int Comparisons { get; private set; }

    private static readonly HashSet<string> Relational = new() { "LT", "LTE", "GT", "GTE", "EQ", "NEQ" };

    /// <param name="condition">Tokens of one guard.</param>
    /// <param name="lets">LET definitions visible to the guard, keyed by unprefixed name.</param>
    /// <param name="inputs">Declared inputs by name.</param>
    public void Add(IReadOnlyList<string> condition, IReadOnlyDictionary<string, IReadOnlyList<string>> lets, IReadOnlyDictionary<string, InputDecl> inputs)
    {
        for (int i = 1; i + 1 < condition.Count; i++)
        {
            if (!Relational.Contains(condition[i])) continue;
            var (ident, literal) = IsNumber(condition[i + 1], out var r) ? (condition[i - 1], r)
                                 : IsNumber(condition[i - 1], out var l) ? (condition[i + 1], l)
                                 : (null, 0m);
            if (ident is null) continue;
            Comparisons++;
            if (!Resolve(ident, literal, lets, inputs)) Unresolved.Add($"{ident} {condition[i]} {literal}");
        }
    }

    private bool Resolve(string ident, decimal literal, IReadOnlyDictionary<string, IReadOnlyList<string>> lets, IReadOnlyDictionary<string, InputDecl> inputs)
    {
        if (inputs.TryGetValue(ident, out var decl) && decl.Kind == InputKind.Number)
        {
            Set(Numeric, ident).Add(literal);
            return true;
        }
        if (ident.EndsWith("!COUNT") && inputs.TryGetValue(ident[..^6], out var multi) && multi.Kind == InputKind.MultiList)
        {
            Set(Count, multi.Name).Add((int)literal);
            return true;
        }
        if (DateProperty(ident, inputs) is { } dp)
        {
            Set(Date, dp.Name).Add((dp.Unit, literal));
            return true;
        }
        if (lets.TryGetValue(ident, out var tokens))
        {
            var props = tokens.Select(t => DateProperty(t, inputs)).Where(p => p is not null).Distinct().ToList();
            var others = tokens.Where(t => inputs.ContainsKey(t)).ToList();
            if (props.Count == 1 && others.Count == 0)
            {
                Set(Date, props[0]!.Value.Name).Add((props[0]!.Value.Unit, literal));
                return true;
            }
        }
        return false;
    }

    private static (string Name, DateUnit Unit)? DateProperty(string token, IReadOnlyDictionary<string, InputDecl> inputs)
    {
        foreach (var (suffix, unit) in new[] { ("!MONTHSTONOW_FROMLASTDAY", DateUnit.Months), ("!MONTHSTONOW", DateUnit.Months), ("!YEARSTONOW", DateUnit.Years), ("!DAYSTONOW", DateUnit.Days) })
        {
            if (token.EndsWith(suffix) && inputs.TryGetValue(token[..^suffix.Length], out var d) && d.Kind == InputKind.Date)
                return (d.Name, unit);
        }
        return null;
    }

    private static HashSet<T> Set<T>(Dictionary<string, HashSet<T>> map, string key) =>
        map.TryGetValue(key, out var s) ? s : map[key] = new HashSet<T>();

    private static bool IsNumber(string token, out decimal value) =>
        decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
}

/// <summary>One input as the vector generator sees it: the values to try and the preferred one.</summary>
public sealed record Parameter(string Name, InputKind Kind, IReadOnlyList<object> Values, int Preferred);

/// <summary>
/// Builds the values tried for one input of a schedule: every list choice, booleans both ways,
/// the default and the declared bounds, and every guard literal with its neighbours (±1).
/// </summary>
public static class InputSpace
{
    public static Parameter Build(InputDecl decl, Guards guards, DateOnly today)
    {
        var values = new List<object>();
        object preferred = decl.Default;

        switch (decl.Kind)
        {
            case InputKind.Boolean:
                values.Add(false); values.Add(true);
                break;

            case InputKind.List:
                values.AddRange(decl.Choices);
                if (!decl.Choices.Contains((string)preferred) && decl.Choices.Count > 0) preferred = decl.Choices[0];
                break;

            case InputKind.MultiList:
            {
                var choices = decl.Choices.ToList();
                var sets = new List<string[]> { ((string[])decl.Default).Where(choices.Contains).ToArray(), Array.Empty<string>(), choices.ToArray() };
                sets.AddRange(choices.Select(c => new[] { c }));
                if (guards.Count.TryGetValue(decl.Name, out var counts))
                    foreach (var b in counts)
                        foreach (var k in new[] { b - 1, b, b + 1 })
                            if (k >= 0 && k <= choices.Count) sets.Add(choices.Take(k).ToArray());
                foreach (var s in sets)
                    if (!values.Any(v => ((string[])v).SequenceEqual(s))) values.Add(s);
                preferred = values[0];
                break;
            }

            case InputKind.Number:
            {
                var set = new SortedSet<decimal> { decl.Min, decl.Max, (decimal)decl.Default };
                if (guards.Numeric.TryGetValue(decl.Name, out var bounds))
                    foreach (var b in bounds) { set.Add(b - 1); set.Add(b); set.Add(b + 1); }
                values.AddRange(set.Where(v => v >= decl.Min && v <= decl.Max).Cast<object>());
                if (!values.Contains(preferred)) preferred = values[0];
                break;
            }

            case InputKind.Date:
            {
                var set = new SortedSet<DateOnly> { decl.DateMin, decl.DateMax, today };
                if (guards.Date.TryGetValue(decl.Name, out var bounds))
                {
                    foreach (var (unit, b) in bounds)
                    {
                        foreach (var k in new[] { b - 1, b, b + 1 })
                        {
                            var d = unit switch
                            {
                                Guards.DateUnit.Days => today.AddDays(-(int)k),
                                Guards.DateUnit.Months => today.AddMonths(-(int)k),
                                _ => today.AddYears(-(int)k)
                            };
                            set.Add(d);
                            if (unit != Guards.DateUnit.Days && k == b) { set.Add(d.AddDays(-1)); set.Add(d.AddDays(1)); }
                        }
                    }
                }
                values.AddRange(set.Where(v => v >= decl.DateMin && v <= decl.DateMax).Cast<object>());
                preferred = values.Contains(today) ? today : values[^1];
                break;
            }

            case InputKind.Amount:
                values.Add(decl.Default);
                break;
        }

        if (values.Count == 0) values.Add(decl.Default);
        var index = values.FindIndex(v => Equals(v, preferred) || (v is string[] a && preferred is string[] p && a.SequenceEqual(p)));
        return new Parameter(decl.Name, decl.Kind, values, Math.Max(0, index));
    }
}
