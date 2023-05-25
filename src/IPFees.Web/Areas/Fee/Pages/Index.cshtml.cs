using IPFees.Core.Model;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;

namespace IPFees.Web.Areas.Fee.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<JurisdictionInfo> Fees { get; set; }
        [BindProperty] public IEnumerable<string> Errors { get; set; }

        private readonly IFeeRepository feeRepository;

        public IndexModel(IFeeRepository feeRepository)
        {
            this.feeRepository = feeRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var DbJur = await feeRepository.GetJurisdictions();
            Fees = DbJur.OrderBy(o => o.Name).ThenBy(o=>o.Category);
            return Page();
        }
    }
}
