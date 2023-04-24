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
        //[BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        [BindProperty] public IList<ParsedVariableViewModel> Vars { get; set; }
        [BindProperty] public IEnumerable<string> Errors { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly IOfficialFee officialFee;
        private readonly ILogger<DataCollectModel> _logger;

        public DataCollectModel(IJurisdictionRepository jurisdictionRepository, IOfficialFee officialFee, ILogger<DataCollectModel> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.officialFee = officialFee;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(IEnumerable<Guid> Id)
        {
            var VarMap = new Dictionary<string, DslVariable>();
            // Get selected jurisdictions
            foreach (var id in Id)
            {
                // For each jurisdiction, get the inputs that need to be displayed to the user
                var res = await officialFee.GetVariables(id);
                if (res is OfficialFeeResultFail)
                {
                    var rf = res as OfficialFeeResultFail;
                    foreach (var e in rf.Errors) Errors.Append(e);
                    return Page();
                }
                // Store parsed variables and remove duplicates
                foreach (var pv in (res as OfficialFeeParseSuccess).ParsedVariables)
                {
                    VarMap.Add(pv.Name, pv);
                }
            }
            Vars = VarMap.Values.Select(pv => new ParsedVariableViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {            
            return RedirectToPage("Result");
        }

        private async Task PopulateItems()
        {

        }
    }    
}
