namespace IPFees.Web.Data
{
    public class CurrencySettings
    {
        public const string SectionName = nameof(CurrencySettings);
        public string[] AllowedCurrencies { get; set; }
        public double CurrencyMarkup { get; set; }
    }
}
