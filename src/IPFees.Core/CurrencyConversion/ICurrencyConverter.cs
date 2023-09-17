namespace IPFees.Core.CurrencyConversion
{
    public interface ICurrencyConverter
    {
        ExchangeRateResponse Response { get; set; }

        decimal ConvertCurrency(decimal Amount, string SourceCurrency, string TargetCurrency);
        IEnumerable<(string, string)> GetCurrencies();        
    }
}