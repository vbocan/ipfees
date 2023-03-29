using IPFees.Evaluator;
using IPFees.Web.Data;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        [BindProperty] public IEnumerable<string> Errors { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public IndexModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var DbJur = await jurisdictionRepository.GetJurisdictions();
            Jurisdictions = DbJur.OrderByDescending(o => o.LastUpdatedOn);
            return Page();
        }
    }
}
