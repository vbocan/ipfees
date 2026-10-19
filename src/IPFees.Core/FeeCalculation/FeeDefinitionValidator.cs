using IPFees.Core.Model;
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
        private readonly IFeeRepository? FeeRepository;

        public FeeDefinitionValidator(IModuleRepository moduleRepository, IFeeScriptComposer composer, IDslCalculator calculator, VerificationBudget? budget = null, IFeeRepository? feeRepository = null)
        {
            ModuleRepository = moduleRepository;
            Composer = composer;
            Calculator = calculator;
            Budget = budget ?? new VerificationBudget();
            FeeRepository = feeRepository;
        }

        public async Task<FeeValidationReport> ValidateAsync(string sourceCode, IEnumerable<Guid> referencedModules)
        {
            // Read modules fresh: the author may have just changed one in another tab.
            var modules = await ModuleRepository.GetModules();
            return Validate(sourceCode, "this fee", referencedModules, modules);
        }

        public async Task<ModuleValidationReport> ValidateModuleAsync(Guid? moduleId, string sourceCode, bool autoRun)
        {
            var stored = (await ModuleRepository.GetModules()).ToList();

            // The module on its own: a base must at least parse and type-check.
            var own = Validate(sourceCode, "this module", Array.Empty<Guid>(), Array.Empty<ModuleInfo>());

            if (FeeRepository is null || !own.CanBeStored)
            {
                return new ModuleValidationReport(own, Array.Empty<DependentValidation>());
            }

            // The schedules the change reaches: every one naming the module, or every one at all
            // when the module runs before each calculation.
            var id = moduleId ?? Guid.NewGuid();
            var wasAutoRun = stored.FirstOrDefault(m => m.Id == id)?.AutoRun ?? false;
            var dependents = (await FeeRepository.GetFees())
                .Where(f => autoRun || wasAutoRun || f.ReferencedModules.Contains(id))
                .ToList();

            var after = stored.Where(m => m.Id != id).ToList();
            var current = stored.FirstOrDefault(m => m.Id == id);
            after.Add(current is null
                ? new ModuleInfo(id, "this module", "", sourceCode, autoRun, DateTime.UtcNow)
                : current with { SourceCode = sourceCode, AutoRun = autoRun });

            var results = new List<DependentValidation>();
            foreach (var fee in dependents)
            {
                var before = Validate(fee.SourceCode, fee.Name, fee.ReferencedModules, stored);
                var changed = Validate(fee.SourceCode, fee.Name, fee.ReferencedModules, after);
                results.Add(new DependentValidation(fee.Name, before, changed));
            }

            return new ModuleValidationReport(own, results);
        }

        private FeeValidationReport Validate(string sourceCode, string label, IEnumerable<Guid> referencedModules, IEnumerable<ModuleInfo> modules)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return FeeValidationReport.FromParseErrors(new[] { "The definition is empty." });
            }

            var (script, errors) = Composer.Compose(sourceCode, label, referencedModules, modules);
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
                var results = new ScheduleVerifier(Budget).Verify(
                    Calculator.GetInputs(), Calculator.GetFees(), Calculator.GetVerifications());

                // Errors raised while running the directives themselves, e.g. a VERIFY that
                // names a fee the script does not define, are reported with the parse errors
                // because they too mean the definition is not what its author intended.
                return new FeeValidationReport(
                    results.Errors,
                    Array.Empty<string>(),
                    results.CompletenessFailures.Select(VerificationNarrative.Describe).ToList(),
                    results.MonotonicityFailures.Select(VerificationNarrative.Describe).ToList(),
                    declared,
                    results.Unfinished);
            }
            catch (Exception ex)
            {
                return FeeValidationReport.FromParseErrors(new[] { $"Verification could not be completed: {ex.Message}" });
            }
        }
    }
}
