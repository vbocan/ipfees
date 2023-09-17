namespace IPFees.Core.CurrencyConversion
{
    public interface IExchangeRateFetcher
    {
        Task<ExchangeResponse> FetchCurrencyExchangeData();        
    }
}