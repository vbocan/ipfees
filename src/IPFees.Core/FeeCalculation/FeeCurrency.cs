using System.Text.RegularExpressions;
using IPFLang.Parser;

namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// Works out which currency a fee's amount is in.
    ///
    /// The engine checks currencies when a schedule is loaded but evaluates a currency literal
    /// to its bare number, so a computed amount carries no currency of its own. A schedule
    /// declares one currency through RETURN Currency, yet a single fee may still be written in
    /// another: an agent's charge quoted as <c>100&lt;EUR&gt;</c> in a schedule that otherwise
    /// prices in SGD. Treating every amount as being in the schedule's currency would count
    /// that charge as 100 SGD.
    ///
    /// The rules mirror the engine's type checker. A fee that declares its return currency is in
    /// that currency. Otherwise its yields and LET bindings decide: a currency literal, an
    /// AMOUNT input, or a CONVERT to a named currency each fix the currency, and an expression
    /// built only from plain numbers is in the schedule's currency.
    /// </summary>
    public static class FeeCurrency
    {
        private static readonly Regex CurrencyLiteral = new(@"^-?\d+(?:\.\d+)?<([A-Z]{3})>$", RegexOptions.Compiled);
        private static readonly Regex CurrencyCode = new(@"^[A-Z]{3}$", RegexOptions.Compiled);

        /// <summary>
        /// The currency of <paramref name="fee"/>'s amount.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The fee's yields resolve to more than one currency, so its amount cannot be converted
        /// as a single figure.
        /// </exception>
        public static string Resolve(DslFee fee, IEnumerable<DslInput> inputs, string scheduleCurrency)
        {
            if (fee.ReturnCurrency is not null && CurrencyCode.IsMatch(fee.ReturnCurrency))
            {
                return fee.ReturnCurrency;
            }

            var amountInputs = inputs.OfType<DslInputAmount>()
                .GroupBy(i => i.Name)
                .ToDictionary(g => g.Key, g => g.First().Currency);

            // LET bindings may themselves carry a currency; yields refer to them by their short name.
            var lets = new Dictionary<string, IReadOnlyCollection<string>>();
            foreach (var let in fee.Vars)
            {
                var found = CurrenciesIn(let.ValueTokens, amountInputs, lets);
                lets[let.Name] = found;
                var dot = let.Name.IndexOf('.');
                if (dot >= 0) lets[let.Name[(dot + 1)..]] = found;
            }

            var currencies = new HashSet<string>();
            foreach (var yield in Yields(fee))
            {
                currencies.UnionWith(CurrenciesIn(yield.Values, amountInputs, lets));
            }

            return currencies.Count switch
            {
                0 => scheduleCurrency,
                1 => currencies.First(),
                _ => throw new InvalidOperationException(
                    $"'{fee.Name}' yields amounts in {string.Join(" and ", currencies.OrderBy(c => c))}; " +
                    "convert them to one currency with CONVERT or split the fee.")
            };
        }

        private static IEnumerable<DslFeeYield> Yields(DslFee fee)
        {
            foreach (var item in fee.Cases)
            {
                switch (item)
                {
                    case DslFeeCase c:
                        foreach (var y in c.Yields) yield return y;
                        break;
                    case DslFeeYield y:
                        yield return y;
                        break;
                }
            }
        }

        private static IReadOnlyCollection<string> CurrenciesIn(
            IEnumerable<string> expression,
            IReadOnlyDictionary<string, string> amountInputs,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> lets)
        {
            var tokens = expression.ToList();
            var found = new HashSet<string>();

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                // CONVERT(amount, [source,] target) is in the target currency whatever it converts,
                // so its arguments are skipped rather than inspected.
                if (token.Equals("CONVERT", StringComparison.OrdinalIgnoreCase) && i + 1 < tokens.Count && tokens[i + 1] == "(")
                {
                    var close = MatchingParen(tokens, i + 1);
                    if (close > i + 1 && CurrencyCode.IsMatch(tokens[close - 1])) found.Add(tokens[close - 1]);
                    i = close;
                    continue;
                }

                var literal = CurrencyLiteral.Match(token);
                if (literal.Success)
                {
                    found.Add(literal.Groups[1].Value);
                    continue;
                }

                if (amountInputs.TryGetValue(token, out var inputCurrency))
                {
                    found.Add(inputCurrency);
                    continue;
                }

                if (lets.TryGetValue(token, out var letCurrencies))
                {
                    found.UnionWith(letCurrencies);
                }
            }

            return found;
        }

        private static int MatchingParen(IReadOnlyList<string> tokens, int open)
        {
            var depth = 0;
            for (var i = open; i < tokens.Count; i++)
            {
                if (tokens[i] == "(") depth++;
                else if (tokens[i] == ")" && --depth == 0) return i;
            }
            return tokens.Count - 1;
        }
    }
}
