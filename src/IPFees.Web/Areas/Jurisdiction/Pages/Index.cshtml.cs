using IPFees.Evaluator;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using IPFees.Core.Repository;
using IPFees.Core.Model;
using IPFees.Core.Enum;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        private readonly IJurisdictionRepository jurisdictionRepository;        

        public IndexModel(IJurisdictionRepository jurisdictionRepository)
        {            
            this.jurisdictionRepository = jurisdictionRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var DbJur = await jurisdictionRepository.GetJurisdictions();
            Jurisdictions = DbJur.OrderBy(o => o.Name);
            return Page();
        }
    }    
}
