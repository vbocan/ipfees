using System.Globalization;
using IPFLang.CurrencyConversion;

namespace IPFees.Core.CurrencyConversion
{
    /// <summary>
    /// The table of exchange rates the application ships for when the rate service cannot be
    /// reached (<c>wwwroot/data/exchange_rates.csv</c>).
    ///
    /// The first line is a header. Every other line is <c>CODE;rate</c>, where the rate is how many
    /// units of the currency one euro buys, written with a decimal point and no digit grouping,
    /// whatever the culture of the machine that reads it. A line that does not have that shape is
    /// rejected rather than guessed at: a rate read with the wrong separator is off by orders of
    /// magnitude and converts every fee in that currency wrongly without any error.
    /// </summary>
    public static class FallbackExchangeRates
    {
        public static (Dictionary<string, decimal> Rates, List<string> Rejected) Parse(IEnumerable<string> lines)
        {
            var rates = new Dictionary<string, decimal>(StringComparer.Ordinal);
            var rejected = new List<string>();

            foreach (var raw in lines.Skip(1))
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;

                var parts = line.Split(';');
                if (parts.Length == 2
                    && parts[0].Length == 3 && parts[0].All(char.IsAsciiLetterUpper)
                    && decimal.TryParse(parts[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var rate)
                    && rate > 0)
                {
                    rates[parts[0]] = rate;
                }
                else
                {
                    rejected.Add(line);
                }
            }

            return (rates, rejected);
        }

        /// <summary>
        /// Read the table at <paramref name="path"/> as a stale response. Rejected lines are passed
        /// to <paramref name="onRejected"/> so the caller can log them.
        /// </summary>
        public static ExchangeRateResponse Load(string path, Action<string>? onRejected = null)
        {
            if (!File.Exists(path))
            {
                return new ExchangeRateResponse(ResponseStatus.Invalid, $"Default exchange rates file not found at {path}", new Dictionary<string, decimal>(), DateTime.Now);
            }

            var (rates, rejected) = Parse(File.ReadAllLines(path));
            foreach (var line in rejected) onRejected?.Invoke(line);
            return new ExchangeRateResponse(ResponseStatus.Stale, "Loaded from default CSV file", rates, DateTime.Now);
        }
    }
}
