namespace IPFees.Core
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertCurrency(decimal amount, string baseCurrency, string targetCurrency);
    }
}