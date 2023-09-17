using System.Text.Json;

namespace IPFees.Core.CurrencyConversion
{
    public class CurrencyConverter
    {
        private static readonly Currency[] Currencies = {
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

        /// <summary>
        /// Perform currency conversion
        /// </summary>
        /// <param name="exchangeResponse">Information retrieved from the exchange server (see ExchangeRateFetcher)</param>
        /// <param name="Amount">Amount in source currency</param>
        /// <param name="SourceCurrency">Source currency (e.g. USD)</param>
        /// <param name="TargetCurrency">Target currency (e.g. RON)</param>
        /// <returns>Amount expressed in the target currency</returns>
        /// <exception cref="Exception">If the exhange data is not valid, an exception will be thrown</exception>
        /// <exception cref="ArgumentException">If the currencies are not known, an exception will be thrown</exception>
        public static decimal ConvertCurrency(ExchangeResponse exchangeResponse, decimal Amount, string SourceCurrency, string TargetCurrency)
        {
            // Check whether we've fetched the currency exchange rates
            if (!exchangeResponse.ServerResponseValid) throw new Exception("No currency information available");
            // Check whether the source and target currencies are actually in the exchange rate data
            if (!exchangeResponse.ExchangeRates.ContainsKey(SourceCurrency)) throw new ArgumentException($"Currency {SourceCurrency} does not exist", nameof(SourceCurrency));
            if (!exchangeResponse.ExchangeRates.ContainsKey(TargetCurrency)) throw new ArgumentException($"Currency {TargetCurrency} does not exist", nameof(TargetCurrency));
            // Compute currency exhange rate
            decimal SourceExchangeRate = exchangeResponse.ExchangeRates[SourceCurrency];
            decimal TargetExchangeRate = exchangeResponse.ExchangeRates[TargetCurrency];
            var AmountInTargetCurrency = (Amount / SourceExchangeRate) * TargetExchangeRate;
            return AmountInTargetCurrency;
        }

        /// <summary>
        /// Get an enumeration of currency symbols and their names
        /// </summary>
        /// <returns>An enumeration of tuples of symbol and name</returns>
        public static IEnumerable<(string, string)> GetCurrencies() => Currencies.OrderBy(o => o.Symbol).Select(s => (s.Symbol, s.Name));

        private record Currency(string Symbol, string Name);
    }
}
