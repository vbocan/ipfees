using IPFees.Core.CurrencyConversion;

namespace IPFees.Core.SharedDataExchange
{
    public class SharedExchangeRateData : ISharedExchangeRateData
    {
        public ExchangeResponse Response { get; set; }

        public decimal ConvertCurrency(decimal Amount, string SourceCurrency, string TargetCurrency)
        {
            return CurrencyConverter.ConvertCurrency(Response, Amount, SourceCurrency, TargetCurrency);
        }

        public IEnumerable<(string, string)> GetCurrencies()
        {
            return CurrencyConverter.GetCurrencies();
        }
    }
}
