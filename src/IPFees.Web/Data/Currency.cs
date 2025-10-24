namespace IPFees.Web.Data
{
    public class CurrencySettings
    {
        public const string SectionName = nameof(CurrencySettings);
        public string[] AllowedCurrencies { get; set; } = null!;
        public decimal CurrencyMarkup { get; set; }
    }
}
