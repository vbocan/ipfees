using IPFees.Evaluator;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using IPFees.Core.Repository;
using IPFees.Core.Model;
using IPFees.Core.Enum;

namespace IPFees.Web.Areas.Currency.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SharedExchangeRateData serd;
        public IEnumerable<(string, string)> Currencies { get; set; }
        public DateTime ExchangeDataLastUpdatedOn { get; set; }

        public IndexModel(SharedExchangeRateData serd)
        {
            this.serd = serd;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Currencies = serd.GetCurrencies();            
            ExchangeDataLastUpdatedOn = serd.Response.LastUpdatedOn;
            var amount = serd.ConvertCurrency(100, "USD", "RON");
            return Page();
        }
    }    
}
