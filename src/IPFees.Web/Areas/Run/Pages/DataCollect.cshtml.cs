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
    public class DataCollectModel : PageModel
    {
        [BindProperty] public Guid[] SelectedJurisdictions { get; set; }
        [BindProperty] public IList<ParsedVariableViewModel> Vars { get; set; }
        [BindProperty] public IList<string> Errors { get; set; }

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
            var VarMap = new Dictionary<string, DslVariable>();
            // Get selected jurisdictions
            foreach (var id in Id)
            {
                // For each jurisdiction, get the inputs that need to be displayed to the user
                var res = await officialFee.GetVariables(id);
                if (res is OfficialFeeResultFail)
                {

                    Errors.Add($"Failed to process jurisdiction {id}.");
                }
                else
                {
                    // Store parsed variables and remove duplicates
                    foreach (var pv in (res as OfficialFeeParseSuccess).ParsedVariables)
                    {
                        VarMap.Add(pv.Name, pv);
                    }
                }
            }
            if (Errors.Any())
            {
                TempData["Errors"] = Errors;
                return RedirectToPage("Error");
            }
            else
            {
                Vars = VarMap.Values.Select(pv => new ParsedVariableViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
                return Page();
            }
        }
    }
}
