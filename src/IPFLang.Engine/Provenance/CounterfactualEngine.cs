using IPFLang.CurrencyConversion;
using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFLang.Provenance
{
    /// <summary>
    /// Generates counterfactual explanations showing what would happen
    /// with different input values.
    /// </summary>
    public class CounterfactualEngine
    {
        private readonly ICurrencyConverter? _currencyConverter;

        public CounterfactualEngine(ICurrencyConverter? currencyConverter = null)
        {
            _currencyConverter = currencyConverter;
        }

        /// <summary>
        /// Generate counterfactuals for the given computation
        /// </summary>
        public List<Counterfactual> GenerateCounterfactuals(
            IEnumerable<DslFee> fees,
            IEnumerable<DslInput> inputs,
            IEnumerable<IPFValue> currentValues,
            decimal currentTotal)
        {
            var counterfactuals = new List<Counterfactual>();
            var feeList = fees.ToList();

            foreach (var candidate in GetCandidates(inputs, currentValues))
            {
                // Compute total with modified values
                var altTotal = ComputeTotal(feeList, candidate.Values.ToList());

                // Only include if there's a meaningful difference
                if (altTotal != currentTotal)
                {
                    counterfactuals.Add(new Counterfactual(
                        candidate.InputName,
                        candidate.OriginalValue,
                        candidate.AlternativeValue,
                        currentTotal,
                        altTotal));
                }
            }

            return counterfactuals;
        }

        /// <summary>
        /// The single-input variations counterfactuals are drawn from, before any is computed:
        /// the other choices of a list, the bounds and midpoint of a number, the opposite of a
        /// boolean, and double and half an amount, each with the input values that result.
        /// A caller that totals fees differently from <see cref="GenerateCounterfactuals"/>
        /// (per currency, say) computes these itself.
        /// </summary>
        public static IEnumerable<CounterfactualCandidate> GetCandidates(
            IEnumerable<DslInput> inputs,
            IEnumerable<IPFValue> currentValues)
        {
            var valueList = currentValues.ToList();

            foreach (var input in inputs.ToList())
            {
                foreach (var (altValue, altIPFValue) in GetAlternativeValues(input, valueList))
                {
                    var modifiedValues = valueList
                        .Where(v => v.Name != input.Name)
                        .Append(altIPFValue)
                        .ToList();

                    yield return new CounterfactualCandidate(
                        input.Name,
                        GetCurrentValue(input.Name, valueList),
                        altValue,
                        modifiedValues);
                }
            }
        }

        /// <summary>
        /// Get alternative values to try for an input
        /// </summary>
        private static IEnumerable<(object Value, IPFValue IPFValue)> GetAlternativeValues(
            DslInput input,
            List<IPFValue> currentValues)
        {
            var current = currentValues.FirstOrDefault(v => v.Name == input.Name);
            if (current == null) yield break;

            switch (input)
            {
                case DslInputBoolean boolInput:
                    var currentBool = ((IPFValueBoolean)current).Value;
                    yield return (!currentBool, new IPFValueBoolean(input.Name, !currentBool));
                    break;

                case DslInputList listInput:
                    var currentSymbol = ((IPFValueString)current).Value;
                    foreach (var item in listInput.Items)
                    {
                        if (item.Symbol != currentSymbol)
                        {
                            yield return (item.Symbol, new IPFValueString(input.Name, item.Symbol));
                        }
                    }
                    break;

                case DslInputNumber numInput:
                    var currentNum = ((IPFValueNumber)current).Value;
                    // Try boundary values
                    if (currentNum != numInput.MinValue)
                        yield return (numInput.MinValue, new IPFValueNumber(input.Name, numInput.MinValue));
                    if (currentNum != numInput.MaxValue)
                        yield return (numInput.MaxValue, new IPFValueNumber(input.Name, numInput.MaxValue));
                    // Try middle value
                    var mid = (numInput.MinValue + numInput.MaxValue) / 2;
                    if (mid != currentNum && mid != numInput.MinValue && mid != numInput.MaxValue)
                        yield return (mid, new IPFValueNumber(input.Name, mid));
                    break;

                case DslInputAmount amountInput:
                    var currentAmount = ((IPFValueAmount)current).Value;
                    // Try doubled and halved
                    if (currentAmount > 0)
                    {
                        yield return (currentAmount * 2, new IPFValueAmount(input.Name, currentAmount * 2, amountInput.Currency));
                        yield return (currentAmount / 2, new IPFValueAmount(input.Name, currentAmount / 2, amountInput.Currency));
                    }
                    break;
            }
        }

        /// <summary>
        /// Get current value for an input
        /// </summary>
        private static object GetCurrentValue(string inputName, List<IPFValue> values)
        {
            var value = values.FirstOrDefault(v => v.Name == inputName);
            return value switch
            {
                IPFValueNumber n => n.Value,
                IPFValueBoolean b => b.Value,
                IPFValueString s => s.Value,
                IPFValueDate d => d.Value,
                IPFValueAmount a => a.Value,
                _ => "unknown"
            };
        }

        /// <summary>
        /// Compute total fees for given inputs
        /// </summary>
        private decimal ComputeTotal(List<DslFee> fees, List<IPFValue> values)
        {
            decimal total = 0;

            foreach (var fee in fees)
            {
                var allVars = new List<IPFValue>(values);

                // Evaluate LET variables
                foreach (var letVar in fee.Vars)
                {
                    var val = DslEvaluator.EvaluateExpression(
                        letVar.ValueTokens.ToArray(),
                        allVars,
                        fee.Name,
                        _currencyConverter);
                    allVars.Add(new IPFValueNumber(letVar.Name, val));
                }

                // Evaluate cases and yields
                foreach (var item in fee.Cases)
                {
                    if (item is DslFeeCase feeCase)
                    {
                        var caseResult = !feeCase.Condition.Any() ||
                            DslEvaluator.EvaluateLogic(feeCase.Condition.ToArray(), allVars, fee.Name, _currencyConverter);

                        if (caseResult)
                        {
                            foreach (var yield in feeCase.Yields)
                            {
                                var yieldResult = !yield.Condition.Any() ||
                                    DslEvaluator.EvaluateLogic(yield.Condition.ToArray(), allVars, fee.Name, _currencyConverter);

                                if (yieldResult)
                                {
                                    total += DslEvaluator.EvaluateExpression(
                                        yield.Values.ToArray(),
                                        allVars,
                                        fee.Name,
                                        _currencyConverter);
                                }
                            }
                        }
                    }
                    else if (item is DslFeeYield directYield)
                    {
                        var yieldResult = !directYield.Condition.Any() ||
                            DslEvaluator.EvaluateLogic(directYield.Condition.ToArray(), allVars, fee.Name, _currencyConverter);

                        if (yieldResult)
                        {
                            total += DslEvaluator.EvaluateExpression(
                                directYield.Values.ToArray(),
                                allVars,
                                fee.Name,
                                _currencyConverter);
                        }
                    }
                }
            }

            return total;
        }
    }

    /// <summary>
    /// One input set to another value, with every other input as it was.
    /// </summary>
    /// <param name="InputName">The input that is varied.</param>
    /// <param name="OriginalValue">Its value as given.</param>
    /// <param name="AlternativeValue">The value tried in its place.</param>
    /// <param name="Values">The input values with <paramref name="InputName"/> replaced.</param>
    public record CounterfactualCandidate(
        string InputName,
        object OriginalValue,
        object AlternativeValue,
        IReadOnlyList<IPFValue> Values);
}
