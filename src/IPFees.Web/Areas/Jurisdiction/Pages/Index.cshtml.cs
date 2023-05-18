using IPFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;

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
            Jurisdictions = DbJur.OrderBy(o => o.Name).ThenBy(o=>o.Category);
            return Page();
        }
    }
}
