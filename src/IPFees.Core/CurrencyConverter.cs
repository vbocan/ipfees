using MongoDB.Bson.IO;
using System.Text.Json;
using ThirdParty.Json.LitJson;

namespace IPFees.Core
{
    /// <summary>
    /// Uses https://exchangerate-api.com
    /// </summary>
    public class CurrencyConverter : ICurrencyConverter
    {
        private string APIKey { get; set; }
        private readonly string APIURL = "https://v6.exchangerate-api.com/v6/[APIKEY]/latest/[BASECURRENCY]";
        private readonly Currency[] Currencies = {
            new Currency("AED","UAE Dirham"),
            new Currency("AFN","Afghan Afghani"),
            new Currency("ALL","Albanian Lek"),
            new Currency("AMD","Armenian Dram"),
            new Currency("ANG","Netherlands Antillian Guilder"),
            new Currency("AOA","Angolan Kwanza"),
            new Currency("ARS","Argentine Peso"),
            new Currency("AUD","Australian Dollar"),
            new Currency("AWG","Aruban Florin"),
            new Currency("AZN","Azerbaijani Manat"),
            new Currency("BAM","Bosnia and Herzegovina Mark"),
            new Currency("BBD","Barbados Dollar"),
            new Currency("BDT","Bangladeshi Taka"),
            new Currency("BGN","Bulgarian Lev"),
            new Currency("BHD","Bahraini Dinar"),
            new Currency("BIF","Burundian Franc"),
            new Currency("BMD","Bermudian Dollar"),
            new Currency("BND","Brunei Dollar"),
            new Currency("BOB","Bolivian Boliviano"),
            new Currency("BRL","Brazilian Real"),
            new Currency("BSD","Bahamian Dollar"),
            new Currency("BTN","Bhutanese Ngultrum"),
            new Currency("BWP","Botswana Pula"),
            new Currency("BYN","Belarusian Ruble"),
            new Currency("BZD","Belize Dollar"),
            new Currency("CAD","Canadian Dollar"),
            new Currency("CDF","Congolese Franc"),
            new Currency("CHF","Swiss Franc"),
            new Currency("CLP","Chilean Peso"),
            new Currency("CNY","Chinese Renminbi"),
            new Currency("COP","Colombian Peso"),
            new Currency("CRC","Costa Rican Colon"),
            new Currency("CUP","Cuban Peso"),
            new Currency("CVE","Cape Verdean Escudo"),
            new Currency("CZK","Czech Koruna"),
            new Currency("DJF","Djiboutian Franc"),
            new Currency("DKK","Danish Krone"),
            new Currency("DOP","Dominican Peso"),
            new Currency("DZD","Algerian Dinar"),
            new Currency("EGP","Egyptian Pound"),
            new Currency("ERN","Eritrean Nakfa"),
            new Currency("ETB","Ethiopian Birr"),
            new Currency("EUR","Euro"),
            new Currency("FJD","Fiji Dollar"),
            new Currency("FKP","Falkland Islands Pound"),
            new Currency("FOK","Faroese Króna"),
            new Currency("GBP","Pound Sterling"),
            new Currency("GEL","Georgian Lari"),
            new Currency("GGP","Guernsey Pound"),
            new Currency("GHS","Ghanaian Cedi"),
            new Currency("GIP","Gibraltar Pound"),
            new Currency("GMD","Gambian Dalasi"),
            new Currency("GNF","Guinean Franc"),
            new Currency("GTQ","Guatemalan Quetzal"),
            new Currency("GYD","Guyanese Dollar"),
            new Currency("HKD","Hong Kong Dollar"),
            new Currency("HNL","Honduran Lempira"),
            new Currency("HRK","Croatian Kuna"),
            new Currency("HTG","Haitian Gourde"),
            new Currency("HUF","Hungarian Forint"),
            new Currency("IDR","Indonesian Rupiah"),
            new Currency("ILS","Israeli New Shekel"),
            new Currency("IMP","Manx Pound"),
            new Currency("INR","Indian Rupee"),
            new Currency("IQD","Iraqi Dinar"),
            new Currency("IRR","Iranian Rial"),
            new Currency("ISK","Icelandic Króna"),
            new Currency("JEP","Jersey Pound"),
            new Currency("JMD","Jamaican Dollar"),
            new Currency("JOD","Jordanian Dinar"),
            new Currency("JPY","Japanese Yen"),
            new Currency("KES","Kenyan Shilling"),
            new Currency("KGS","Kyrgyzstani Som"),
            new Currency("KHR","Cambodian Riel"),
            new Currency("KID","Kiribati Dollar"),
            new Currency("KMF","Comorian Franc"),
            new Currency("KRW","South Korean Won"),
            new Currency("KWD","Kuwaiti Dinar"),
            new Currency("KYD","Cayman Islands Dollar"),
            new Currency("KZT","Kazakhstani Tenge"),
            new Currency("LAK","Lao Kip"),
            new Currency("LBP","Lebanese Pound"),
            new Currency("LKR","Sri Lanka Rupee"),
            new Currency("LRD","Liberian Dollar"),
            new Currency("LSL","Lesotho Loti"),
            new Currency("LYD","Libyan Dinar"),
            new Currency("MAD","Moroccan Dirham"),
            new Currency("MDL","Moldovan Leu"),
            new Currency("MGA","Malagasy Ariary"),
            new Currency("MKD","Macedonian Denar"),
            new Currency("MMK","Burmese Kyat"),
            new Currency("MNT","Mongolian Tögrög"),
            new Currency("MOP","Macanese Pataca"),
            new Currency("MRU","Mauritanian Ouguiya"),
            new Currency("MUR","Mauritian Rupee"),
            new Currency("MVR","Maldivian Rufiyaa"),
            new Currency("MWK","Malawian Kwacha"),
            new Currency("MXN","Mexican Peso"),
            new Currency("MYR","Malaysian Ringgit"),
            new Currency("MZN","Mozambican Metical"),
            new Currency("NAD","Namibian Dollar"),
            new Currency("NGN","Nigerian Naira"),
            new Currency("NIO","Nicaraguan Córdoba"),
            new Currency("NOK","Norwegian Krone"),
            new Currency("NPR","Nepalese Rupee"),
            new Currency("NZD","New Zealand Dollar"),
            new Currency("OMR","Omani Rial"),
            new Currency("PAB","Panamanian Balboa"),
            new Currency("PEN","Peruvian Sol"),
            new Currency("PGK","Papua New Guinean Kina"),
            new Currency("PHP","Philippine Peso"),
            new Currency("PKR","Pakistani Rupee"),
            new Currency("PLN","Polish Złoty"),
            new Currency("PYG","Paraguayan Guaraní"),
            new Currency("QAR","Qatari Riyal"),
            new Currency("RON","Romanian Leu"),
            new Currency("RSD","Serbian Dinar"),
            new Currency("RUB","Russian Ruble"),
            new Currency("RWF","Rwandan Franc"),
            new Currency("SAR","Saudi Riyal"),
            new Currency("SBD","Solomon Islands Dollar"),
            new Currency("SCR","Seychellois Rupee"),
            new Currency("SDG","Sudanese Pound"),
            new Currency("SEK","Swedish Krona"),
            new Currency("SGD","Singapore Dollar"),
            new Currency("SHP","Saint Helena Pound"),
            new Currency("SLE","Sierra Leonean Leone"),
            new Currency("SOS","Somali Shilling"),
            new Currency("SRD","Surinamese Dollar"),
            new Currency("SSP","South Sudanese Pound"),
            new Currency("STN","São Tomé and Príncipe Dobra"),
            new Currency("SYP","Syrian Pound"),
            new Currency("SZL","Eswatini Lilangeni"),
            new Currency("THB","Thai Baht"),
            new Currency("TJS","Tajikistani Somoni"),
            new Currency("TMT","Turkmenistan Manat"),
            new Currency("TND","Tunisian Dinar"),
            new Currency("TOP","Tongan Paʻanga"),
            new Currency("TRY","Turkish Lira"),
            new Currency("TTD","Trinidad and Tobago Dollar"),
            new Currency("TVD","Tuvaluan Dollar"),
            new Currency("TWD","New Taiwan Dollar"),
            new Currency("TZS","Tanzanian Shilling"),
            new Currency("UAH","Ukrainian Hryvnia"),
            new Currency("UGX","Ugandan Shilling"),
            new Currency("USD","United States Dollar"),
            new Currency("UYU","Uruguayan Peso"),
            new Currency("UZS","Uzbekistani So'm"),
            new Currency("VES","Venezuelan Bolívar Soberano"),
            new Currency("VND","Vietnamese Đồng"),
            new Currency("VUV","Vanuatu Vatu"),
            new Currency("WST","Samoan Tālā"),
            new Currency("XAF","Central African CFA Franc"),
            new Currency("XCD","East Caribbean Dollar"),
            new Currency("XDR","Special Drawing Rights"),
            new Currency("XOF","West African CFA franc"),
            new Currency("XPF","CFP Franc"),
            new Currency("YER","Yemeni Rial"),
            new Currency("ZAR","South African Rand"),
            new Currency("ZMW","Zambian Kwacha"),
            new Currency("ZWL","Zimbabwean Dollar"),
            };
        public CurrencyConverter(string APIKey)
        {
            this.APIKey = APIKey;
        }
        /// <summary>
        /// Convert a monetary amount from a currency to another
        /// </summary>
        /// <param name="Amount">Amount to convert</param>
        /// <param name="BaseCurrencySymbol">Source currency</param>
        /// <param name="TargetCurrencySymbol">Target currency</param>
        /// <example>
        /// decimal amount = 1;
        /// string baseCurrency = "EUR";
        /// string targetCurrency = "RON";
        /// decimal convertedAmount = await ConvertCurrency(amount, baseCurrency, targetCurrency);
        /// Console.WriteLine($"{amount} {baseCurrency} is equivalent to {convertedAmount} {targetCurrency}");
        /// </example>
        /// <returns>The amount converted to target currency</returns>
        public async Task<decimal> ConvertCurrency(decimal Amount, string BaseCurrencySymbol, string TargetCurrencySymbol)
        {
            string url = GetAPIKey(BaseCurrencySymbol);

            using HttpClient client = new();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                //decimal conversionRate = jsonDocument.RootElement.GetProperty("conversion_rates").GetProperty(TargetCurrencySymbol).GetDecimal();

                var jsonConversionRates = jsonDocument.RootElement.GetProperty("conversion_rates");
                ConversionRates conversionRates = JsonSerializer.Deserialize<ConversionRates>(jsonConversionRates);


                decimal convertedAmount = Amount * conversionRate;

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

        public IEnumerable<(string, string)> GetCurrencies() => Currencies.OrderBy(o => o.Symbol).Select(s => (s.Symbol, s.Name));

        private string GetAPIKey(string BaseCurrency) => APIURL.Replace("[APIKEY]", APIKey).Replace("[BASECURRENCY]", BaseCurrency);
    }

    public record Currency(string Symbol, string Name);

    public class ConversionRates
    {
        public Dictionary<string, decimal> conversion_rates { get; set; }

        public KeyValuePair<string, decimal>[] Rates
        {
            get { return conversion_rates.ToArray(); }
        }
    }
}
