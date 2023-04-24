using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;
using static IPFees.Core.OfficialFee;

namespace IPFees.Web.Areas.Run.Pages
{
    public class IndexModel : PageModel
    {        
        [BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        [BindProperty] public IList<JurisdictionViewModel> SelectedJurisdictions { get; set; }
        //indProperty] public IEnumerable<string> Errors { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly IOfficialFee officialFee;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IJurisdictionRepository jurisdictionRepository, IOfficialFee officialFee, ILogger<IndexModel> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.officialFee = officialFee;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var DbJur = await jurisdictionRepository.GetJurisdictions();
            Jurisdictions = DbJur.OrderBy(o => o.Name);
            SelectedJurisdictions = DbJur.OrderBy(o=>o.Name).Select(s => new JurisdictionViewModel(s.Id, s.Name, s.Description, true)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get selected jurisdictions
            var SelJur = SelectedJurisdictions.Where(w => w.Checked).Select(s => s.Id.ToString()).ToList();
            // TODO: For each selected jurisdiction, get the needed inputs
            // TODO: Remove duplicate inputs, such that one given input is only displayed once
            // TODO: Perform calculation and display results
            return Page1();
        }
    }

    public record JurisdictionViewModel(Guid Id, string Name, string Description, bool Checked);
}
