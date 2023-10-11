using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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