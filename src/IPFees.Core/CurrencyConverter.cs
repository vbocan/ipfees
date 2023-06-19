using System.Text.Json;

namespace IPFees.Core
{
    /// <summary>
    /// Uses https://exchangerate-api.com
    /// </summary>
    public class CurrencyConverter : ICurrencyConverter
    {
        private string APIKey { get; set; }
        private readonly string APIURL = "https://v6.exchangerate-api.com/v6/[APIKEY]/latest/[BASECURRENCY]";

        public CurrencyConverter(string APIKey)
        {
            this.APIKey = APIKey;
        }
        /// <summary>
        /// Convert a monetary amount from a currency to another
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="baseCurrency">Source currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <example>
        /// decimal amount = 1;
        /// string baseCurrency = "EUR";
        /// string targetCurrency = "RON";
        /// decimal convertedAmount = await ConvertCurrency(amount, baseCurrency, targetCurrency);
        /// Console.WriteLine($"{amount} {baseCurrency} is equivalent to {convertedAmount} {targetCurrency}");
        /// </example>
        /// <returns>The amount converted to target currency</returns>
        public async Task<decimal> ConvertCurrency(decimal amount, string baseCurrency, string targetCurrency)
        {
            string url = GetAPIKey(baseCurrency);

            using HttpClient client = new();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                decimal conversionRate = jsonDocument.RootElement.GetProperty("conversion_rates").GetProperty(targetCurrency).GetDecimal();
                decimal convertedAmount = amount * conversionRate;

                return convertedAmount;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Failed to retrieve exchange rates: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return 0m;
        }

        private string GetAPIKey(string BaseCurrency) => APIURL.Replace("[APIKEY]", APIKey).Replace("[BASECURRENCY]", BaseCurrency);
    }
}
