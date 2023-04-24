using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using static IPFees.Core.OfficialFee;

namespace IPFees.Web.Areas.Run.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        [BindProperty] public IList<JurisdictionViewModel> SelectedJurisdictions { get; set; }        

        private readonly IJurisdictionRepository jurisdictionRepository;        
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IJurisdictionRepository jurisdictionRepository, ILogger<IndexModel> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;            
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Jurisdictions = await jurisdictionRepository.GetJurisdictions();
            SelectedJurisdictions = Jurisdictions.OrderBy(o => o.Name).Select(s => new JurisdictionViewModel(s.Id, s.Name, s.Description, true)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var SelJur = await SelectedJurisdictions.ToAsyncEnumerable().Where(w => w.Checked).Select(s => s.Id).ToListAsync();
            return RedirectToPage("DataCollect", new { area = "Run", Id = SelJur });
        }
    }

    public record JurisdictionViewModel(Guid Id, string Name, string Description, bool Checked);
}
