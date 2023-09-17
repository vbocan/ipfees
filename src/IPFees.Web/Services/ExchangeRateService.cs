using IPFees.Core.CurrencyConversion;
using IPFees.Core.SharedDataExchange;
using IPFees.Web.Data;
using SharpCompress.Readers;

namespace IPFees.Web.Services
{
    public class ExchangeRateService : BackgroundService
    {
        private readonly TimeSpan ExchangeRateDelaySuccess = TimeSpan.FromHours(6);
        private readonly TimeSpan ExchangeRateDelayFail = TimeSpan.FromSeconds(30);
        private readonly IExchangeRateFetcher currencyConverter;
        private readonly ISharedExchangeRateData sharedExchangeRateData;
        private readonly ILogger<ExchangeRateService> logger;

        public ExchangeRateService(IExchangeRateFetcher currencyConverter, ISharedExchangeRateData sharedExchangeRateData, ILogger<ExchangeRateService> logger)
        {
            this.sharedExchangeRateData = sharedExchangeRateData;
            this.currencyConverter = currencyConverter;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation($"Fetching exchange rates");
                var response = await currencyConverter.FetchCurrencyExchangeData();
                if (response.ServerResponseValid)
                {
                    logger.LogInformation($"Fetched {response.ExchangeRates.Count} exchange rates");
                }
                else
                {
                    logger.LogError($"Failed to fetch exchange rates: {response.ServerReason}");
                }
                // Store fetched data                
                sharedExchangeRateData.Response = response;
                var delay = response.ServerResponseValid ? ExchangeRateDelaySuccess : ExchangeRateDelayFail;
                logger.LogInformation($"Exchange rate service going to sleep for {delay.ToString(@"hh\:mm\:ss")}");
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
