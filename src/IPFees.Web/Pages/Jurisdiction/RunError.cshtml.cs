using IPFees.Evaluator;
using IPFees.Web.Data;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class RunErrorModel : PageModel
    {        
        [BindProperty] public IEnumerable<string> Errors { get; set; }     
    
        public RunErrorModel()
        {
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Errors = (IEnumerable<string>)ViewData["ParseErrors"];
            return Page();
        }
    }
}
