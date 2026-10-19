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

        /// <summary>
        /// Check a module before it is stored. A module is shared, so the check covers both the
        /// module on its own and every fee definition the change reaches, composed with the
        /// module as it is now and as it would be after the save.
        /// </summary>
        /// <param name="moduleId">The module being edited, or null for a new one.</param>
        Task<ModuleValidationReport> ValidateModuleAsync(Guid? moduleId, string sourceCode, bool autoRun);

        /// <summary>
        /// Check the removal of a module. Every fee definition that names the module (every one
        /// at all, for a module that runs before each calculation) is validated as it stands and
        /// as it would stand with the module gone and the reference dropped. Removing a regional
        /// base is a change to its members as much as editing it is, and is judged the same way.
        /// </summary>
        Task<ModuleValidationReport> ValidateModuleRemovalAsync(Guid moduleId);
    }

    /// <summary>
    /// Outcome of validating a module. A module change is refused when the module itself is
    /// broken or when it introduces a failure into a fee definition that did not have it before.
    /// Failures a dependent already had are reported but do not block: the module's author did
    /// not cause them and cannot fix them from here.
    /// </summary>
    public record ModuleValidationReport(FeeValidationReport Own, IReadOnlyList<DependentValidation> Dependents)
    {
        public IEnumerable<DependentValidation> Broken => Dependents.Where(d => d.Broke);
        public bool MayBeSaved => Own.MayBeSaved && !Broken.Any();
    }

    /// <summary>One fee definition reached by a module change, validated before and after it.</summary>
    public record DependentValidation(string FeeName, FeeValidationReport Before, FeeValidationReport After)
    {
        /// <summary>Findings present after the change that were not there before.</summary>
        public IReadOnlyList<string> NewFailures =>
            Findings(After).Except(Findings(Before)).ToList();

        /// <summary>The change made this definition unsaveable.</summary>
        public bool Broke => !After.MayBeSaved && NewFailures.Count > 0;

        private static IEnumerable<string> Findings(FeeValidationReport r) =>
            r.ParseErrors.Concat(r.TypeErrors).Concat(r.CompletenessFailures).Concat(r.MonotonicityFailures);
    }

    /// <summary>
    /// How long a save is willing to wait for static verification before giving the author an
    /// answer. The budget covers all of a schedule's directives together; a directive still
    /// running when it expires is abandoned and reported as unfinished, while the directives that
    /// did finish keep their verdicts (see <see cref="ScheduleVerifier"/>).
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
    /// <param name="Unfinished">
    /// Directives abandoned at the budget, neither proven nor disproven. The others keep their
    /// verdicts, so a coverage gap is still reported when a monotonicity check overruns.
    /// </param>
    public record FeeValidationReport(
        IReadOnlyList<string> ParseErrors,
        IReadOnlyList<string> TypeErrors,
        IReadOnlyList<string> CompletenessFailures,
        IReadOnlyList<string> MonotonicityFailures,
        int VerificationsRun,
        IReadOnlyList<string>? Unfinished = null)
    {
        public IReadOnlyList<string> UnfinishedDirectives => Unfinished ?? Array.Empty<string>();

        /// <summary>At least one directive was abandoned at the budget.</summary>
        public bool VerificationTimedOut => UnfinishedDirectives.Count > 0;

        /// <summary>A script that cannot be parsed or type-checked must not be stored.</summary>
        public bool CanBeStored => ParseErrors.Count == 0 && TypeErrors.Count == 0;

        /// <summary>
        /// The save gate: parse and type errors block, and so does any directive that finished and
        /// failed. Directives that did not finish do not block, because nothing was established
        /// about them either way.
        /// </summary>
        public bool MayBeSaved => CanBeStored && VerificationsPassed;

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
