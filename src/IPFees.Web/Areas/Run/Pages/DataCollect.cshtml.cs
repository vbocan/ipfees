using IPFees.Core.FeeManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Run.Pages
{
    public class DataCollectModel : PageModel
    {
        [BindProperty] public string[] SelectedJurisdictions { get; set; }
        [BindProperty] public string TargetCurrency { get; set; }
        public IList<InputViewModel> Inputs { get; set; }
        public IEnumerable<GroupViewModel> Groups { get; set; }
        public int[] UncategorizedInputs { get; set; }
        public IEnumerable<string> Errors { get; set; }

        private readonly IJurisdictionFeeManager jurisdictionFeeManager;
        private readonly ILogger<DataCollectModel> _logger;

        public DataCollectModel(IJurisdictionFeeManager jurisdictionFeeManager, ILogger<DataCollectModel> logger)
        {
            this.jurisdictionFeeManager = jurisdictionFeeManager;
            this.Errors = new List<string>();
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string[] Id, string TargetCurrency)
        {
            SelectedJurisdictions = Id;
            this.TargetCurrency = TargetCurrency;

            // For each jurisdiction, get the inputs that need to be displayed to the user
            var (inputs, groups, errs) = jurisdictionFeeManager.GetConsolidatedInputs(SelectedJurisdictions);

            Inputs = inputs.Select(pv => new InputViewModel(pv.Group, pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
            Groups = groups
                .OrderBy(o => o.Weight)
                .ThenBy(p => p.Name)
                .Select(pv => new GroupViewModel(pv.Name, pv.Text, pv.Weight, Inputs.Select((s, index) => new { Value = s, Index = index }).Where(x => x.Value.Group == pv.Name).Select(x => x.Index).ToArray()));
            UncategorizedInputs = Inputs
                .Select((s, index) => new { Value = s, Index = index })
                .Where(x => x.Value.Group.Equals(string.Empty))
                .Select(x => x.Index).ToArray();
            Errors = errs.Select(s => $"[{s.FeeName}] - {s.FeeName} (Internal Error)");
            return Page();
        }
    }

    public record GroupViewModel(string Name, string Text, int Weight, int[] InputIndexes);
}
