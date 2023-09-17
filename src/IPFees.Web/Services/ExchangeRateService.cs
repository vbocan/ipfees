using IPFees.Core.CurrencyConversion;

namespace IPFees.Web.Services
{
    public class ExchangeRateService : BackgroundService
    {
        private readonly TimeSpan ExchangeRateDelaySuccess = TimeSpan.FromHours(6);
        private readonly TimeSpan ExchangeRateDelayFail = TimeSpan.FromSeconds(30);
        private readonly IExchangeRateFetcher currencyConverter;
        private readonly ICurrencyConverter sharedExchangeRateData;
        private readonly ILogger<ExchangeRateService> logger;

        public ExchangeRateService(IExchangeRateFetcher currencyConverter, ICurrencyConverter sharedExchangeRateData, ILogger<ExchangeRateService> logger)
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
                if (response.ResponseValid)
                {
                    logger.LogInformation($"Fetched {response.ExchangeRates.Count} exchange rates");
                }
                else
                {
                    logger.LogError($"Failed to fetch exchange rates: {response.Reason}");
                }
                // Store fetched data                
                sharedExchangeRateData.Response = response;
                var delay = response.ResponseValid ? ExchangeRateDelaySuccess : ExchangeRateDelayFail;
                logger.LogInformation($"Exchange rate service going to sleep for {delay.ToString(@"hh\:mm\:ss")}");
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
