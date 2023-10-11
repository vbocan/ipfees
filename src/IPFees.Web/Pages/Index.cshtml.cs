using IPFees.Calculator;
using IPFees.Web.Areas.Fee.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IPFees.Core.Repository;
using IPFees.Core.CurrencyConversion;
using IPFees.Web.Data;

namespace IPFees.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return RedirectToPage("Index", new { area = "Run" });
        }
    }
}