using IPFees.Core.FeeCalculation;
using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFees.Core.FeeManager
{
    public interface IJurisdictionFeeManager
    {
        (IEnumerable<DslInput>, IEnumerable<DslGroup>, IEnumerable<FeeResultFail>) GetConsolidatedInputs(IEnumerable<string> JurisdictionNames);
        Task<TotalFeeInfo> Calculate(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues, string TargetCurrency, decimal CurrencyMarkup);

        /// <summary>
        /// Run the static verification directives declared by every fee definition behind the
        /// given jurisdictions. Reports which schedules prove complete and monotonic, and which
        /// declared nothing to prove.
        /// </summary>
        IEnumerable<FeeVerificationInfo> Verify(IEnumerable<string> JurisdictionNames);

        /// <summary>
        /// Compute the given jurisdictions and return the reasoning behind every amount:
        /// which rules fired, which did not and why, and what each input was worth at the time.
        /// </summary>
        IEnumerable<FeeExplanation> Explain(IEnumerable<string> JurisdictionNames, IList<IPFValue> InputValues, bool IncludeAlternatives = false);
    }

    /// <summary>
    /// Verification outcome for one stored fee definition.
    /// </summary>
    /// <param name="DirectivesDeclared">
    /// How many VERIFY directives the definition declares. Zero means nothing was proven,
    /// which is different from everything having passed.
    /// </param>
    /// <param name="TimedOut">
    /// True when the analysis was abandoned before finishing. The directives were neither
    /// proven nor disproven, so <paramref name="Passed"/> carries no meaning.
    /// </param>
    public record FeeVerificationInfo(
        string Jurisdiction,
        string FeeName,
        string Category,
        int DirectivesDeclared,
        bool Passed,
        IReadOnlyList<string> CompletenessFailures,
        IReadOnlyList<string> MonotonicityFailures,
        IReadOnlyList<string> Errors,
        bool TimedOut = false);
}
