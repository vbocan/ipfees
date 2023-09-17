using IPFees.Core.CurrencyConversion;
using IPFees.Web.Data;
using SharpCompress.Readers;

namespace IPFees.Web.Services
{
    public class ExchangeRateService : BackgroundService
    {
        private readonly TimeSpan ExchangeRateDelay = TimeSpan.FromHours(6);
        private readonly IExchangeRateFetcher currencyConverter;
        private readonly SharedExchangeRateData sharedExchangeRateData;
        private readonly ILogger<ExchangeRateService> logger;

        public ExchangeRateService(IExchangeRateFetcher currencyConverter, SharedExchangeRateData sharedExchangeRateData, ILogger<ExchangeRateService> logger)
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
                logger.LogInformation($"Exchange rate service going to sleep for {ExchangeRateDelay.ToString(@"hh\:mm\:ss")}");
                await Task.Delay(ExchangeRateDelay, stoppingToken);
            }
        }
    }
}
