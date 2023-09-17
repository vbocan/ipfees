using IPFees.Core.CurrencyConversion;

namespace IPFees.Core.SharedDataExchange
{
    public interface ISharedExchangeRateData
    {
        ExchangeResponse Response { get; set; }

        decimal ConvertCurrency(decimal Amount, string SourceCurrency, string TargetCurrency);
        IEnumerable<(string, string)> GetCurrencies();
    }
}