using IPFees.Calculator;
using IPFees.Core;
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
    public class DataCollectModel : PageModel
    {
        [BindProperty] public Guid[] SelectedJurisdictions { get; set; }
        [BindProperty] public IList<InputViewModel> Inputs { get; set; }
        [BindProperty] public IEnumerable<string> Errors { get; set; }        

        private readonly IOfficialFee officialFee;
        private readonly ILogger<DataCollectModel> _logger;

        public DataCollectModel(IOfficialFee officialFee, ILogger<DataCollectModel> logger)
        {
            this.officialFee = officialFee;
            this.Errors = new List<string>();
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(Guid[] Id)
        {
            SelectedJurisdictions = Id;

            // For each jurisdiction, get the inputs that need to be displayed to the user
            var (inputs, errs) = await officialFee.GetConsolidatedInputs(Id);

            Inputs = inputs.Select(pv => new InputViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
            Errors = errs.Select(s => $"[{s.JurisdictionName}] - {s.JurisdictionName}");
            return Page();
        }
    }
}
