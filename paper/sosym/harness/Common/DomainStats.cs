using IPFLang.Analysis;
using N = IPFLang.Parser;

namespace Harness.Common;

/// <summary>
/// Sizes of the input spaces the two checkers work over, computed the way the engine
/// enumerates them (IPFLang.Analysis) but without enumerating anything. Sizes are
/// reported as log10 because most overflow a 64-bit integer.
/// </summary>
public static class DomainStats
{
    public static double Log10Cardinality(InputDomain d) => d switch
    {
        MultiListDomain m => m.Symbols.Count * Math.Log10(2),
        AmountDomain => double.PositiveInfinity,
        _ => Math.Log10(Math.Max(1, (double)(d.Cardinality ?? 1)))
    };

    /// <summary>Full input space of a schedule: the Cartesian product of every declared domain.</summary>
    public static double Log10InputSpace(IEnumerable<InputDomain> domains) => domains.Sum(Log10Cardinality);

    /// <summary>
    /// What CompletenessChecker.CheckFeeCompleteness looks at: only the domains its conditions
    /// reference, enumerated exhaustively up to 10^6 combinations and sampled above that.
    /// </summary>
    public static (double Log10Size, string Method, int Variables) Completeness(N.DslFee fee, IReadOnlyList<InputDomain> domains)
    {
        var referenced = new ConditionExtractor().ExtractFeeConditions(fee)
            .SelectMany(c => c.Condition.GetReferencedVariables()).ToHashSet();
        var relevant = domains.Where(d => referenced.Contains(d.VariableName)).ToList();
        if (relevant.Count == 0) return (0, "unconditional", 0);
        var log = Log10InputSpace(relevant);
        return (log, log > 6 ? "sampling" : "exhaustive", relevant.Count);
    }

    /// <summary>
    /// What MonotonicityChecker.CheckMonotonicity looks at: every representative combination of
    /// all other declared inputs (not only those the fee uses), times up to 20 points of the
    /// target input. Returns log10 of the number of fee evaluations it plans.
    /// </summary>
    public static (double Log10Contexts, int Points, double Log10Evaluations) Monotonicity(string target, IReadOnlyList<InputDomain> domains)
    {
        var t = domains.FirstOrDefault(d => d.VariableName == target) as NumericDomain;
        var points = t is null ? 0 : t.GetRepresentativeValues(20).Count();
        var contexts = domains.Where(d => d.VariableName != target).Sum(d => Math.Log10(Math.Max(1, RepresentativeCount(d))));
        return (contexts, points, contexts + Math.Log10(Math.Max(1, points)));
    }

    /// <summary>DomainAnalyzer.GetRepresentativeValues with maxSamples = 5, counted.</summary>
    private static double RepresentativeCount(InputDomain d) => d switch
    {
        NumericDomain n => n.GetRepresentativeValues(5).Count(),
        DateDomain dd => dd.GetRepresentativeValues().Count(),
        AmountDomain a => a.GetValues().Count(),
        MultiListDomain m => Math.Pow(2, m.Symbols.Count),
        _ => (double)(d.Cardinality ?? 1)
    };
}
