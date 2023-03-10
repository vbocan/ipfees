using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static IPFees.Core.OfficialFee;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class DataCollectModel : PageModel
    {
        [BindProperty]
        public IEnumerable<DslVariable> Vars { get; set; }

        private readonly IOfficialFee officialFee;
        private readonly ILogger<IndexModel> _logger;

        public DataCollectModel(IOfficialFee officialFee, ILogger<IndexModel> logger)
        {
            this.officialFee = officialFee;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {            
            var res = await officialFee.GetVariables(id);
            if (!res.IsSuccessfull)
            {
                ViewData["ParseErrors"] = (res as OfficialFeeResultFail).Errors;
                return RedirectToPage("RunError");
            }            
            Vars = (res as OfficialFeeParseSuccess).RequestedVariables;
            return Page();
        }
    }
}
