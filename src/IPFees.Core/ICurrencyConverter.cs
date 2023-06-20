using static IPFees.Core.CurrencyConverter;

namespace IPFees.Core
{
    public interface ICurrencyConverter
    {
        Task<ExchangeResponse> FetchCurrencyExchangeData();
        IEnumerable<(string, string)> GetCurrencies();
    }
}