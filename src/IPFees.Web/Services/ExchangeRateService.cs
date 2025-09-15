using IPFees.Core.CurrencyConversion;
using Microsoft.Extensions.Configuration;

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
                var response = await exchangeRateFetcher.FetchCurrencyExchangeData();
                if (response.Status == ResponseStatus.ResponseOnline)
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
                var delay = response.Status == ResponseStatus.ResponseOnline ? ExchangeRateDelaySuccess : ExchangeRateDelayFail;
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
                if (!File.Exists(csvFilePath))
                {
                    logger.LogError($"Default exchange rates CSV file not found at: {csvFilePath}");
                    return new ExchangeRateResponse(ResponseStatus.Invalid, "Default exchange rates CSV file not found", new Dictionary<string, decimal>(), DateTime.Now);
                }

                var exchangeRates = new Dictionary<string, decimal>();
                var lines = File.ReadAllLines(csvFilePath);

                // Skip header line
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    var parts = line.Split(';');
                    if (parts.Length >= 2)
                    {
                        var currencyCode = parts[0];
                        if (decimal.TryParse(parts[1], out var rate))
                        {
                            exchangeRates[currencyCode] = rate;
                        }
                    }
                }

                logger.LogInformation($"Loaded {exchangeRates.Count} default exchange rates from CSV");
                return new ExchangeRateResponse(ResponseStatus.ResponseStale, "Loaded from default CSV file", exchangeRates, DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error loading default exchange rates from CSV: {ex.Message}");
                return new ExchangeRateResponse(ResponseStatus.Invalid, $"Error loading default exchange rates: {ex.Message}", new Dictionary<string, decimal>(), DateTime.Now);
            }
        }
    }
}
