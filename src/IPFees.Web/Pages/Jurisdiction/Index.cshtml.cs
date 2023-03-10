using IPFees.Evaluator;
using IPFees.Web.Data;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        private readonly IJurisdictionRepository moduleRepository;

        public IndexModel(IJurisdictionRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;            
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Jurisdictions = await moduleRepository.GetJurisdictions();
            return Page();
        }
    }
}
