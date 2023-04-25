using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Run.Pages
{
    public class ErrorModel : PageModel
    {
        [BindProperty] public IEnumerable<string> Errors { get; set; }

        public ErrorModel()
        {
        }
        public async Task<IActionResult> OnGetAsync(IEnumerable<string> err)
        {
            Errors = err;
            return Page();
        }
    }
}
