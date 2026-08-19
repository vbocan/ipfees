using IPFees.Core.Repository;
using IPFLang.Engine;

namespace IPFees.Core.FeeCalculation
{
    /// <inheritdoc cref="IFeeDefinitionValidator"/>
    public class FeeDefinitionValidator : IFeeDefinitionValidator
    {
        private readonly IModuleRepository ModuleRepository;
        private readonly IFeeScriptComposer Composer;
        private readonly IDslCalculator Calculator;
        private readonly VerificationBudget Budget;

        public FeeDefinitionValidator(IModuleRepository moduleRepository, IFeeScriptComposer composer, IDslCalculator calculator, VerificationBudget? budget = null)
        {
            ModuleRepository = moduleRepository;
            Composer = composer;
            Calculator = calculator;
            Budget = budget ?? new VerificationBudget();
        }

        public async Task<FeeValidationReport> ValidateAsync(string sourceCode, IEnumerable<Guid> referencedModules)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return FeeValidationReport.FromParseErrors(new[] { "The fee definition is empty." });
            }

            // Read modules fresh: the author may have just changed one in another tab.
            var modules = await ModuleRepository.GetModules();

            var (script, errors) = Composer.Compose(sourceCode, "this fee", referencedModules, modules);
            if (script is null)
            {
                return FeeValidationReport.FromParseErrors(errors.ToList());
            }

            Calculator.Reset();
            Calculator.LoadParsedScript(script);

            var typeErrors = Calculator.GetTypeErrors().Select(e => e.ToString() ?? "Type error").ToList();
            if (typeErrors.Count > 0)
            {
                return new FeeValidationReport(Array.Empty<string>(), typeErrors, Array.Empty<string>(), Array.Empty<string>(), 0);
            }

            var declared = Calculator.GetVerifications().Count();
            if (declared == 0)
            {
                // Nothing was asked for, so nothing is checked. Saying so is more honest than
                // reporting a clean bill of health the script never earned.
                return new FeeValidationReport(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), 0);
            }

            try
            {
                // The engine exposes no cancellation, so a run that overruns the budget is
                // abandoned rather than stopped. That costs background work but keeps the
                // author's page responsive, which matters more here.
                var running = Task.Run(() => Calculator.RunVerifications());
                if (!running.Wait(Budget.Limit))
                {
                    return new FeeValidationReport(
                        Array.Empty<string>(), Array.Empty<string>(),
                        Array.Empty<string>(), Array.Empty<string>(),
                        declared, VerificationTimedOut: true);
                }

                var results = running.Result;

                var completeness = results.CompletenessReports
                    .Where(r => !r.IsComplete)
                    .Select(VerificationNarrative.Describe)
                    .ToList();

                var monotonicity = results.MonotonicityReports
                    .Where(r => !r.IsMonotonic)
                    .Select(VerificationNarrative.Describe)
                    .ToList();

                // Errors raised while running the directives themselves, e.g. a VERIFY that
                // names a fee the script does not define.
                var directiveErrors = results.Errors.ToList();

                return new FeeValidationReport(
                    directiveErrors,
                    Array.Empty<string>(),
                    completeness,
                    monotonicity,
                    declared);
            }
            catch (Exception ex)
            {
                return FeeValidationReport.FromParseErrors(new[] { $"Verification could not be completed: {ex.Message}" });
            }
        }

    }
}
