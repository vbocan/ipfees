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
                var components = Components(InputValues, Returns);
                return new FeeResultCalculation(fee.Name, fee.Description, TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps, Returns, components);
            }
            catch (Exception ex)
            {
                return new FeeResultFail(fee.Name, fee.Description, new[] { ex.Message });
            }
        }

        /// <summary>
        /// Break the computed totals down by fee, each in its own currency. The totals from
        /// <see cref="IDslCalculator.Compute"/> add amounts as bare numbers, which is only right
        /// when every fee is priced in the schedule's currency.
        /// </summary>
        private IReadOnlyList<FeeComponent>? Components(IList<IPFValue> InputValues, IEnumerable<(string, string)> Returns)
        {
            var scheduleCurrency = Returns
                .Where(r => r.Item1.Equals("Currency", StringComparison.OrdinalIgnoreCase))
                .Select(r => r.Item2)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(scheduleCurrency)) return null;

            var inputs = Calculator.GetInputs().ToList();
            var fees = Calculator.GetFees().GroupBy(f => f.Name).ToDictionary(g => g.Key, g => g.Last());

            return Calculator.ComputeWithProvenance(InputValues).FeeBreakdown
                .Select(b => new FeeComponent(
                    b.FeeName,
                    b.Amount,
                    b.IsOptional,
                    fees.TryGetValue(b.FeeName, out var dsl) ? FeeCurrency.Resolve(dsl, inputs, scheduleCurrency) : scheduleCurrency))
                .ToList();
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
                // Same verifier and budget as the save gate, so the REST endpoint and the editor
                // reach the same verdicts. The endpoint is reachable without credentials, so an
                // unbounded wait is not an option either.
                var results = new ScheduleVerifier(Budget).Verify(
                    Calculator.GetInputs(), Calculator.GetFees(), Calculator.GetVerifications());
                return new FeeResultVerification(fee.Name, fee.Description, results);
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

                return new FeeResultExplanation(fee.Name, fee.Description, Describe(fee, provenance, CurrencyOf));
            }
            catch (Exception ex)
            {
                return new FeeResultFail(fee.Name, fee.Description, new[] { ex.Message });
            }
        }

        /// <summary>
        /// Restate the engine's provenance in the platform's own vocabulary.
        /// </summary>
        private static FeeExplanation Describe(FeeInfo fee, ComputationProvenance provenance, Func<string, string?> currencyOf)
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
                    new Dictionary<string, decimal>(r.LetVariables))).ToList(),
                currencyOf(f.FeeName)))
                .ToList();

            var alternatives = provenance.Counterfactuals.Select(c => new ExplainedAlternative(
                c.InputName,
                c.OriginalValue?.ToString() ?? string.Empty,
                c.AlternativeValue?.ToString() ?? string.Empty,
                c.OriginalTotal,
                c.AlternativeTotal,
                c.Difference)).ToList();

            return new FeeExplanation(
                fee.Id,
                fee.Name,
                fee.Description,
                CurrencyTotal.Of(fees.Select(f => (f.Currency, f.Amount, f.Optional))),
                provenance.InputValues.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty),
                fees,
                alternatives);
        }

        /// <summary>
        /// The currency a fee of the loaded schedule is priced in, or null when the schedule
        /// declares no currency or the fee is not part of it.
        /// </summary>
        private string? CurrencyOf(string feeName)
        {
            var scheduleCurrency = Calculator.GetReturns()
                .Where(r => r.Symbol.Equals("Currency", StringComparison.OrdinalIgnoreCase))
                .Select(r => r.Text)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(scheduleCurrency)) return null;

            var fee = Calculator.GetFees().LastOrDefault(f => f.Name == feeName);
            if (fee is null) return null;

            try { return FeeCurrency.Resolve(fee, Calculator.GetInputs(), scheduleCurrency); }
            catch (InvalidOperationException) { return null; }
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
    /// <param name="TotalMandatoryAmount">Mandatory fees added as bare numbers; meaningful as an amount only when every fee is in the schedule's currency.</param>
    /// <param name="TotalOptionalAmount">Optional fees added the same way.</param>
    /// <param name="Components">
    /// Each fee's amount with the currency it is priced in. Null when the schedule declares no
    /// currency. Convert these, not the totals, when the schedule mixes currencies.
    /// </param>
    public record FeeResultCalculation(string FeeName, string FeeDescription, decimal TotalMandatoryAmount, decimal TotalOptionalAmount, IEnumerable<string> CalculationSteps, IEnumerable<(string, string)> Returns, IReadOnlyList<FeeComponent>? Components = null) : FeeResult();

    /// <summary>One fee's computed amount, in the currency it is priced in.</summary>
    public record FeeComponent(string Name, decimal Amount, bool Optional, string Currency);
    public record FeeResultParse(string FeeName, string FeeDescription, IEnumerable<DslInput> FeeInputs, IEnumerable<DslGroup> FeeGroups) : FeeResult();
    /// <param name="Results">Per-directive verdicts, including those cut off by the budget.</param>
    public record FeeResultVerification(string FeeName, string FeeDescription, ScheduleVerification Results) : FeeResult()
    {
        /// <summary>At least one directive was abandoned at the budget.</summary>
        public bool TimedOut => Results.Unfinished.Count > 0;
    }
    public record FeeResultExplanation(string FeeName, string FeeDescription, FeeExplanation Explanation) : FeeResult();
}
