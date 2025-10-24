using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Run.Pages
{
    public class ErrorModel : PageModel
    {
        [BindProperty] public IEnumerable<string> Errors { get; set; } = null!;

        public ErrorModel()
        {
        }
        public IActionResult OnGet(IEnumerable<string> err)
        {
            Errors = TempData["Errors"] as IEnumerable<string> ?? Enumerable.Empty<string>();
            return Page();
        }
    }
}
