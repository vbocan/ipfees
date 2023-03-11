using IPFees.Evaluator;
using IPFees.Web.Data;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Areas.Run.Pages
{
    public class ErrorModel : PageModel
    {
        [BindProperty] public IEnumerable<string> Errors { get; set; }

        public ErrorModel()
        {
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Errors = (IEnumerable<string>)TempData["Errors"];
            return Page();
        }
    }
}
