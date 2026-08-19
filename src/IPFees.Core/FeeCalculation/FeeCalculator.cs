using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFLang.CurrencyConversion;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;
using IPFLang.Provenance;

namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Executes a stored fee definition against the IPFLang engine.
    ///
    /// A fee definition rarely stands alone: it relies on shared input declarations kept
    /// in modules. Those modules are combined with the fee script through IPFLang's
    /// jurisdiction composition, where each module is a parent of the one after it and the
    /// fee script is the leaf. Later definitions override earlier ones of the same name,
    /// which is what lets a fee script specialise a shared module.
    /// </summary>
    public class FeeCalculator : IFeeCalculator
    {
        private readonly IDslCalculator Calculator;
        private readonly IFeeScriptComposer Composer;
        private readonly VerificationBudget Budget;
        private readonly IEnumerable<FeeInfo> Fees;
        private readonly IEnumerable<ModuleInfo> Modules;

        public FeeCalculator(IFeeRepository fee, IModuleRepository module, IDslCalculator calculator, IFeeScriptComposer composer, VerificationBudget? budget = null, ICurrencyConverter? currencyConverter = null)
        {
            Calculator = calculator;
            Composer = composer;
            Budget = budget ?? new VerificationBudget();
            Fees = fee.GetFees().Result;
            Modules = module.GetModules().Result;

            // The engine owns the arithmetic behind the CONVERT operator but carries no exchange
            // rates of its own. Handing it the rates this deployment holds is what makes a
            // schedule written with CONVERT executable; without it the operator has nothing to
            // work from.
            if (currencyConverter is not null)
            {
                Calculator.SetCurrencyConverter(currencyConverter);
            }
        }

        private FeeInfo? GetFeeById(Guid Id) => Fees.SingleOrDefault(w => w.Id.Equals(Id));

        /// <summary>
        /// Compute the specified fee
        /// </summary>
        /// <param name="FeeId">Fee Id</param>
        /// <param name="InputValues">Calculation parameters</param>
        public FeeResult Calculate(Guid FeeId, IList<IPFValue> InputValues)
        {
            var (fee, failure) = Prepare(FeeId);
            if (failure is not null) return failure;

            try
            {
                var (TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns) = Calculator.Compute(InputValues);
                return new FeeResultCalculation(fee.Name, fee.Description, TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns);
            }
            catch (Exception ex)
            {
                return new FeeResultFail(fee.Name, fee.Description, new[] { ex.Message });
            }
        }

        /// <summary>
        /// Get the inputs needed for the specified fee
        /// </summary>
        /// <param name="FeeId">Fee Id</param>
        /// <returns>List of inputs needed for invoking the fee calculation</returns>
        public FeeResult GetInputs(Guid FeeId)
        {
            var (fee, failure) = Prepare(FeeId);
            if (failure is not null) return failure;

            return new FeeResultParse(fee.Name, fee.Description, Calculator.GetInputs(), Calculator.GetGroups());
        }

        /// <summary>
        /// Run the VERIFY directives declared by the fee and its modules. This surfaces
        /// IPFLang's static completeness and monotonicity analysis to callers that never
        /// touch the DSL directly.
        /// </summary>
        /// <param name="FeeId">Fee Id</param>
        public FeeResult Verify(Guid FeeId)
        {
            var (fee, failure) = Prepare(FeeId);
            if (failure is not null) return failure;

            try
            {
                // Monotonicity checking does not finish in usable time on wide schedules, and the
                // engine offers no cancellation, so an overrunning run is abandoned rather than
                // stopped. Without this a caller waits forever; the REST endpoint is reachable
                // without credentials, so forever is not an option.
                var running = Task.Run(() => Calculator.RunVerifications());
                if (!running.Wait(Budget.Limit))
                {
                    return new FeeResultVerification(fee.Name, fee.Description, new VerificationResults(), TimedOut: true);
                }

                return new FeeResultVerification(fee.Name, fee.Description, running.Result);
            }
            catch (Exception ex)
            {
                return new FeeResultFail(fee.Name, fee.Description, new[] { ex.Message });
            }
        }

        /// <summary>
        /// Compute the fee and record why each amount arose.
        ///
        /// Where <see cref="Calculate"/> returns totals, this returns the reasoning behind them:
        /// which yields fired, which did not and on what condition, and what the inputs were at
        /// each step. Asking for alternatives additionally reports what the total would have
        /// been had a single input differed.
        /// </summary>
        /// <param name="FeeId">Fee Id</param>
        /// <param name="InputValues">Calculation parameters</param>
        /// <param name="IncludeAlternatives">Also compute counterfactual totals for each input.</param>
        public FeeResult Explain(Guid FeeId, IList<IPFValue> InputValues, bool IncludeAlternatives = false)
        {
            var (fee, failure) = Prepare(FeeId);
            if (failure is not null) return failure;

            try
            {
                var provenance = IncludeAlternatives
                    ? Calculator.ComputeWithCounterfactuals(InputValues)
                    : Calculator.ComputeWithProvenance(InputValues);

                return new FeeResultExplanation(fee.Name, fee.Description, Describe(fee, provenance));
            }
            catch (Exception ex)
            {
                return new FeeResultFail(fee.Name, fee.Description, new[] { ex.Message });
            }
        }

        /// <summary>
        /// Restate the engine's provenance in the platform's own vocabulary.
        /// </summary>
        private static FeeExplanation Describe(FeeInfo fee, ComputationProvenance provenance)
        {
            var fees = provenance.FeeProvenances.Select(f => new ExplainedFee(
                f.FeeName,
                f.IsOptional,
                f.TotalAmount,
                f.Records.Select(r => new ExplainedStep(
                    r.Expression,
                    r.Contribution,
                    r.DidContribute,
                    DescribeCondition(r),
                    r.ReferencedInputs.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty),
                    new Dictionary<string, decimal>(r.LetVariables))).ToList()))
                .ToList();

            var alternatives = provenance.Counterfactuals.Select(c => new ExplainedAlternative(
                c.InputName,
                c.OriginalValue?.ToString() ?? string.Empty,
                c.AlternativeValue?.ToString() ?? string.Empty,
                c.OriginalTotal,
                c.AlternativeTotal,
                c.Difference)).ToList();

            return new FeeExplanation(
                fee.Name,
                fee.Description,
                provenance.TotalMandatory,
                provenance.TotalOptional,
                provenance.InputValues.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty),
                fees,
                alternatives);
        }

        /// <summary>
        /// Render the guards on a yield as the schedule author wrote them.
        /// </summary>
        private static string? DescribeCondition(ProvenanceRecord record)
        {
            var guards = new List<string>();
            if (!string.IsNullOrEmpty(record.CaseCondition)) guards.Add($"CASE {record.CaseCondition}");
            if (!string.IsNullOrEmpty(record.YieldCondition)) guards.Add($"IF {record.YieldCondition}");
            return guards.Count == 0 ? null : string.Join(" / ", guards);
        }

        /// <summary>
        /// Resolve a fee to its composed script and load it into the calculator.
        /// Returns the fee metadata, plus a failure result when composition did not succeed.
        /// </summary>
        private (FeeInfo Fee, FeeResult? Failure) Prepare(Guid FeeId)
        {
            Calculator.Reset();
            var fee = GetFeeById(FeeId) ?? throw new NotSupportedException($"Fee '{FeeId}' does not exist.");

            try
            {
                var (script, errors) = Composer.Compose(fee.SourceCode, $"fee:{fee.Name}", fee.ReferencedModules, Modules);
                if (script is null)
                {
                    return (fee, new FeeResultFail(fee.Name, fee.Description, errors));
                }

                Calculator.LoadParsedScript(script);
            }
            catch (Exception ex)
            {
                return (fee, new FeeResultFail(fee.Name, fee.Description, new[] { ex.Message }));
            }

            return (fee, null);
        }
    }

    public abstract record FeeResult();
    public record FeeResultFail(string FeeName, string FeeDescription, IEnumerable<string> Errors) : FeeResult();
    public record FeeResultCalculation(string FeeName, string FeeDescription, decimal TotalMandatoryAmount, decimal TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns) : FeeResult();
    public record FeeResultParse(string FeeName, string FeeDescription, IEnumerable<DslInput> FeeInputs, IEnumerable<DslGroup> FeeGroups) : FeeResult();
    /// <param name="TimedOut">
    /// True when verification was abandoned at the budget, so the directives were neither
    /// proven nor disproven and <paramref name="Results"/> is empty.
    /// </param>
    public record FeeResultVerification(string FeeName, string FeeDescription, VerificationResults Results, bool TimedOut = false) : FeeResult();
    public record FeeResultExplanation(string FeeName, string FeeDescription, FeeExplanation Explanation) : FeeResult();
}
