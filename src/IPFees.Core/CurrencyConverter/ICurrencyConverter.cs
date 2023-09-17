using static IPFees.Core.CurrencyConversion.CurrencyConverter;

namespace IPFees.Core.CurrencyConversion
{
    public interface ICurrencyConverter
    {
        Task FetchCurrencyExchangeData();
        IEnumerable<(string, string)> GetCurrencies();
        decimal ConvertCurrency(decimal Amount, string SourceCurrency, string TargetCurrency);
    }
}