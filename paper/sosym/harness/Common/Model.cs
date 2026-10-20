using System.Globalization;
using N = IPFLang.Parser;

namespace Harness.Common;

public enum InputKind { Boolean, List, MultiList, Number, Date, Amount }

/// <summary>
/// An input declaration reduced to what the vector generator needs: kind, choices, bounds and
/// default, whatever IPFLang record it was declared with.
/// </summary>
public sealed record InputDecl(
    string Name,
    InputKind Kind,
    IReadOnlyList<string> Choices,
    decimal Min,
    decimal Max,
    object Default,
    DateOnly DateMin,
    DateOnly DateMax)
{
    public static InputDecl From(N.DslInput input) => input switch
    {
        N.DslInputBoolean b => new(b.Name, InputKind.Boolean, Array.Empty<string>(), 0, 1, b.DefaultValue, default, default),
        N.DslInputList l => new(l.Name, InputKind.List, l.Items.Select(i => i.Symbol).ToList(), 0, 0, l.DefaultSymbol, default, default),
        N.DslInputListMultiple m => new(m.Name, InputKind.MultiList, m.Items.Select(i => i.Symbol).ToList(), 0, 0, m.DefaultSymbols.ToArray(), default, default),
        N.DslInputNumber n => new(n.Name, InputKind.Number, Array.Empty<string>(), n.MinValue, n.MaxValue, (decimal)n.DefaultValue, default, default),
        N.DslInputDate d => new(d.Name, InputKind.Date, Array.Empty<string>(), 0, 0, d.DefaultValue, d.MinValue, d.MaxValue),
        N.DslInputAmount a => new(a.Name, InputKind.Amount, new[] { a.Currency }, 0, 0, a.DefaultValue, default, default),
        _ => throw new NotSupportedException(input.GetType().Name)
    };
}

/// <summary>Conversion of plain values (bool, decimal, string, string[], DateOnly) to engine inputs.</summary>
public static class Values
{
    public static string Show(object value) => value switch
    {
        bool b => b ? "TRUE" : "FALSE",
        decimal d => d.ToString(CultureInfo.InvariantCulture),
        string s => s,
        string[] a => "{" + string.Join("|", a) + "}",
        DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        _ => value.ToString() ?? ""
    };

    public static IPFLang.Evaluator.IPFValue ToEngine(string name, object value) => value switch
    {
        bool b => new IPFLang.Evaluator.IPFValueBoolean(name, b),
        decimal d => new IPFLang.Evaluator.IPFValueNumber(name, d),
        string s => new IPFLang.Evaluator.IPFValueString(name, s),
        string[] a => new IPFLang.Evaluator.IPFValueStringList(name, a.ToList()),
        DateOnly d => new IPFLang.Evaluator.IPFValueDate(name, d),
        _ => throw new NotSupportedException(value.GetType().Name)
    };
}
