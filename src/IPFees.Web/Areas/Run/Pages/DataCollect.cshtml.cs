using IPFees.Calculator;
using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using static IPFees.Core.FeeCalculator;

namespace IPFees.Web.Areas.Run.Pages
{
    public class DataCollectModel : PageModel
    {
        [BindProperty] public string[] SelectedJurisdictions { get; set; }
        [BindProperty] public IList<InputViewModel> Inputs { get; set; }
        [BindProperty] public IEnumerable<string> Errors { get; set; }        

        private readonly IJurisdictionFeeManager jurisdictionFeeManager;
        private readonly ILogger<DataCollectModel> _logger;

        public DataCollectModel(IJurisdictionFeeManager jurisdictionFeeManager, ILogger<DataCollectModel> logger)
        {
            this.jurisdictionFeeManager = jurisdictionFeeManager;
            this.Errors = new List<string>();
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string[] Id)
        {
            SelectedJurisdictions = Id;

            // For each jurisdiction, get the inputs that need to be displayed to the user
            var (inputs, errs) = jurisdictionFeeManager.GetConsolidatedInputs(Id);

            Inputs = inputs.Select(pv => new InputViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
            Errors = errs.Select(s => $"[{s.FeeName}] - {s.FeeName} (Internal Error)");
            return Page();
        }
    }
}
