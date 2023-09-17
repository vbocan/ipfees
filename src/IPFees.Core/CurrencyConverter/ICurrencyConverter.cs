using static IPFees.Core.CurrencyConversion.CurrencyConverter;

namespace IPFees.Core.CurrencyConversion
{
    public interface ICurrencyConverter
    {
        Task<ExchangeResponse> FetchCurrencyExchangeData();
        IEnumerable<(string, string)> GetCurrencies();
    }
}