using Microsoft.Extensions.Configuration;
using IPFees.Core.CurrencyConversion;
using IPFLang.CurrencyConversion;

namespace IPFees.Web.Services
{
    public class ExchangeRateService : BackgroundService
    {
        private readonly TimeSpan ExchangeRateDelaySuccess = TimeSpan.FromHours(6);
        private readonly TimeSpan ExchangeRateDelayFail = TimeSpan.FromMinutes(5);
        private readonly IConfiguration configuration;
        private readonly IExchangeRateFetcher exchangeRateFetcher;
        private readonly ICurrencyConverter currencyConverter;
        private readonly ILogger<ExchangeRateService> logger;

        public ExchangeRateService(IExchangeRateFetcher exchangeRateFetcher, ICurrencyConverter currencyConverter, IConfiguration configuration, ILogger<ExchangeRateService> logger)
        {
            this.configuration = configuration;
            this.currencyConverter = currencyConverter;
            this.exchangeRateFetcher = exchangeRateFetcher;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation($"Fetching exchange rates");
                var response = await exchangeRateFetcher.FetchExchangeRates();
                if (response.Status == ResponseStatus.Online)
                {
                    logger.LogInformation($"Fetched {response.ExchangeRates.Count} exchange rates");
                }
                else
                {
                    logger.LogError($"Failed to fetch exchange rates: {response.Reason}");
                    logger.LogInformation("Loading default exchange rates from CSV file");
                    response = LoadDefaultExchangeRatesFromCsv();
                }
                // Store fetched data
                currencyConverter.Response = response;
                var delay = response.Status == ResponseStatus.Online ? ExchangeRateDelaySuccess : ExchangeRateDelayFail;
                logger.LogInformation($"Exchange rate service going to sleep for {delay.ToString(@"hh\:mm\:ss")}");
                await Task.Delay(delay, stoppingToken);
            }
        }

        private ExchangeRateResponse LoadDefaultExchangeRatesFromCsv()
        {
            try
            {
                var configDataFolder = configuration.GetValue<string>("DataFolder");
                var csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), configDataFolder ?? "wwwroot/data", "exchange_rates.csv");
                var response = FallbackExchangeRates.Load(csvFilePath, line => logger.LogWarning("Ignoring malformed exchange rate line: {Line}", line));
                if (response.Status == ResponseStatus.Invalid)
                {
                    logger.LogError(response.Reason);
                    return response;
                }

                logger.LogInformation($"Loaded {response.ExchangeRates.Count} default exchange rates from CSV");
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error loading default exchange rates from CSV: {ex.Message}");
                return new ExchangeRateResponse(ResponseStatus.Invalid, $"Error loading default exchange rates: {ex.Message}", new Dictionary<string, decimal>(), DateTime.Now);
            }
        }
    }
}
