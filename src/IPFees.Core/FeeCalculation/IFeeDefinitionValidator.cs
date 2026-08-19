namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Checks a fee definition before it is stored, so that an author working in the web
    /// interface finds out about a broken or incomplete schedule while editing it rather
    /// than when somebody later runs a calculation.
    ///
    /// The checks come from the IPFLang engine: syntax and semantics from the parser,
    /// cross-currency arithmetic from the type system, and the VERIFY directives from
    /// static completeness and monotonicity analysis.
    /// </summary>
    public interface IFeeDefinitionValidator
    {
        Task<FeeValidationReport> ValidateAsync(string sourceCode, IEnumerable<Guid> referencedModules);
    }

    /// <summary>
    /// How long the editor is willing to wait for static verification before giving the author
    /// an answer. Completeness checking over the schedules in this corpus finishes in a few
    /// milliseconds, but monotonicity checking walks a numeric domain crossed with every
    /// categorical input, and on wide schedules it does not finish in any useful time. Rather
    /// than hang the page, verification is abandoned at the budget and reported as unfinished.
    /// </summary>
    public class VerificationBudget
    {
        public TimeSpan Limit { get; init; } = TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// Outcome of validating a fee definition.
    /// </summary>
    /// <param name="ParseErrors">Syntax and semantic errors. These block saving.</param>
    /// <param name="TypeErrors">Type errors, most often cross-currency arithmetic. These block saving.</param>
    /// <param name="CompletenessFailures">Fees whose CASE coverage leaves input combinations unhandled.</param>
    /// <param name="MonotonicityFailures">Fees that move the wrong way against an input they declared a direction for.</param>
    /// <param name="VerificationsRun">How many VERIFY directives the script declared.</param>
    /// <param name="VerificationTimedOut">
    /// True when verification was abandoned at the budget. The definition is storable, but its
    /// directives were neither proven nor disproven.
    /// </param>
    public record FeeValidationReport(
        IReadOnlyList<string> ParseErrors,
        IReadOnlyList<string> TypeErrors,
        IReadOnlyList<string> CompletenessFailures,
        IReadOnlyList<string> MonotonicityFailures,
        int VerificationsRun,
        bool VerificationTimedOut = false)
    {
        /// <summary>A script that cannot be parsed or type-checked must not be stored.</summary>
        public bool CanBeStored => ParseErrors.Count == 0 && TypeErrors.Count == 0;

        /// <summary>Every declared VERIFY directive held.</summary>
        public bool VerificationsPassed => CompletenessFailures.Count == 0 && MonotonicityFailures.Count == 0;

        /// <summary>Nothing at all to report.</summary>
        public bool IsClean => CanBeStored && VerificationsPassed;

        /// <summary>The script declared no VERIFY directives, so nothing was statically checked.</summary>
        public bool HasNoVerifications => VerificationsRun == 0;

        public static FeeValidationReport FromParseErrors(IReadOnlyList<string> errors) =>
            new(errors, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), 0);
    }
}
