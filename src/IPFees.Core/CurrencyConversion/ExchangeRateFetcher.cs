using System.Text.Json;

namespace IPFees.Core.CurrencyConversion
{
    /// <summary>
    /// Uses https://exchangerate-api.com
    /// </summary>
    public class ExchangeRateFetcher : IExchangeRateFetcher
    {
        private const string BASECURRENCY = "EUR";
        private string APIKey { get; set; }
        private readonly string APIURL = "https://v6.exchangerate-api.com/v6/[APIKEY]/latest/[BASECURRENCY]";

        public ExchangeRateFetcher(string APIKey)
        {
            this.APIKey = APIKey;
        }

        /// <summary>
        /// Fetch and deserialize exchange data
        /// </summary>        
        public async Task<ExchangeResponse> FetchCurrencyExchangeData()
        {
            string url = APIURL.Replace("[APIKEY]", APIKey).Replace("[BASECURRENCY]", BASECURRENCY);

            using HttpClient client = new();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

                // Check whether the result from the exchange is valid
                var IsSuccessfull = jsonDocument.RootElement.GetProperty("result").GetString()?.Equals("success", StringComparison.InvariantCultureIgnoreCase);
                if (!IsSuccessfull.HasValue || !IsSuccessfull.Value)
                {
                    return new ExchangeResponse(false, "Invalid response from the exchange service", new Dictionary<string, decimal>(), DateTime.Now);
                }
                // Determine the timestamp of the last data update
                var unixLastUpdate = jsonDocument.RootElement.GetProperty("time_last_update_unix").GetDouble();
                var LastUpdatedOn = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixLastUpdate).ToLocalTime();
                // Determine exchange rates
                var jsonRates = jsonDocument.RootElement.GetProperty("conversion_rates").GetRawText();
                var ExchangeRates = ParseExchangeRates(jsonRates);

                return new ExchangeResponse(true, string.Empty, ExchangeRates, LastUpdatedOn);
            }
            catch (Exception ex)
            {
                return new ExchangeResponse(false, $"Unable to fetch exchange data: {ex}", new Dictionary<string, decimal>(), DateTime.Now);
            }
        }

        private static Dictionary<string, decimal> ParseExchangeRates(string jsonString)
        {
            Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>();

            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);

            foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
            {
                string currencyCode = property.Name;
                decimal exchangeRate = property.Value.GetDecimal();

                exchangeRates.Add(currencyCode, exchangeRate);
            }

            return exchangeRates;
        }
    }
    public record ExchangeResponse(bool ServerResponseValid, string ServerReason, Dictionary<string, decimal> ExchangeRates, DateTime LastUpdatedOn);
}
