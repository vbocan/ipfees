namespace IPFees.Core.CurrencyConversion
{
    public interface IExchangeRateFetcher
    {
        Task<ExchangeRateResponse> FetchCurrencyExchangeData();        
    }
}