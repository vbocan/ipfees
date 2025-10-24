using IPFees.Core.CurrencyConversion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Currency.Pages
{    
    public class IndexModel : PageModel
    {
        private readonly ICurrencyConverter serd;
        public IEnumerable<(string, string, decimal?)> Currencies { get; set; } = null!;
        public DateTime ExchangeDataLastUpdatedOn { get; set; }

        public IndexModel(ICurrencyConverter serd)
        {
            this.serd = serd;
        }
        public IActionResult OnGet()
        {
            ExchangeDataLastUpdatedOn = serd.Response.LastUpdatedOn;
            Currencies = GetCurrenciesWithExchangeRate();
            return Page();
        }

        private IEnumerable<(string, string, decimal?)> GetCurrenciesWithExchangeRate()
        {
            foreach (var curr in serd.GetCurrencies())
            {
                var Currency = curr.Item1;
                var Description = curr.Item2;
                decimal? ExchangeRate = null;
                try
                {
                    ExchangeRate = Math.Round(serd.ConvertCurrency(1, Currency, "EUR"), 4);
                }
                catch (Exception) { }
                yield return (Currency, Description, ExchangeRate);
            }
        }
    }
}
