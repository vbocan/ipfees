namespace IPFees.Core
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertCurrency(decimal Amount, string BaseCurrencySymbol, string TargetCurrencySymbol);
        IEnumerable<(string, string)> GetCurrencies();
    }
}