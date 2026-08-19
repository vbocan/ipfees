using IPFLang.CurrencyConversion;

namespace IPFees.Performance.Tests.Fixtures
{
    /// <summary>
    /// Mock currency converter for benchmarks to avoid external API calls
    /// Returns fixed exchange rates for predictable testing
    /// </summary>
    public class MockCurrencyConverter : ICurrencyConverter
    {
        private readonly Dictionary<string, decimal> exchangeRates = new()
        {
            { "USD", 1.0m },
            { "EUR", 0.92m },
            { "GBP", 0.79m },
            { "JPY", 149.50m },
            { "CAD", 1.36m },
            { "CHF", 0.88m }
        };

        public ExchangeRateResponse Response { get; set; }

        public MockCurrencyConverter()
        {
            Response = new ExchangeRateResponse(
                ResponseStatus.Online,
                string.Empty,
                exchangeRates,
                DateTime.Now
            );
        }

        public decimal Convert(decimal Amount, string SourceCurrency, string TargetCurrency)
        {
            if (SourceCurrency == TargetCurrency)
                return Amount;

            var fromRate = exchangeRates.GetValueOrDefault(SourceCurrency, 1.0m);
            var toRate = exchangeRates.GetValueOrDefault(TargetCurrency, 1.0m);

            return Amount * (toRate / fromRate);
        }

        public decimal GetRate(string SourceCurrency, string TargetCurrency)
        {
            if (SourceCurrency == TargetCurrency)
                return 1.0m;

            var fromRate = exchangeRates.GetValueOrDefault(SourceCurrency, 1.0m);
            var toRate = exchangeRates.GetValueOrDefault(TargetCurrency, 1.0m);

            return toRate / fromRate;
        }

        public IEnumerable<(string Code, string Name)> GetCurrencies()
        {
            return exchangeRates.Keys.Select(k => (k, k)).ToList();
        }
    }
}
