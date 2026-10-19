namespace IPFees.Core.FeeCalculation
{
    /// <summary>
    /// The fees of one schedule that are priced in one currency, added up.
    ///
    /// A schedule may price its fees in several currencies: official fees in the office's own,
    /// translation and the agent's charge in EUR. Amounts in different currencies cannot be added
    /// without converting them, so a schedule's total is reported per currency rather than as one
    /// number. A schedule that declares no currency yields a single total with an empty currency.
    /// </summary>
    public record CurrencyTotal(string Currency, decimal Mandatory, decimal Optional)
    {
        public decimal Total => Mandatory + Optional;

        /// <summary>
        /// Group fee amounts by the currency each is priced in, keeping the order in which the
        /// currencies first appear among the fees.
        /// </summary>
        public static IReadOnlyList<CurrencyTotal> Of(IEnumerable<(string? Currency, decimal Amount, bool Optional)> fees) =>
            fees.GroupBy(f => f.Currency ?? string.Empty)
                .Select(g => new CurrencyTotal(
                    g.Key,
                    g.Where(f => !f.Optional).Sum(f => f.Amount),
                    g.Where(f => f.Optional).Sum(f => f.Amount)))
                .ToList();

        /// <summary>Render as "1,200.00 CHF + 700.00 EUR", picking one of the amounts.</summary>
        public static string Format(IEnumerable<CurrencyTotal> totals, Func<CurrencyTotal, decimal> amount) =>
            string.Join(" + ", totals.Select(t => $"{amount(t):N2} {t.Currency}".TrimEnd()));
    }
}
