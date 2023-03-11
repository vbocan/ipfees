using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static IPFees.Core.OfficialFee;

namespace IPFees.Web.Areas.Run.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public List<IPFValue> CollectedVars { get; set; }
        [BindProperty] public IEnumerable<DslVariable> Vars { get; set; }
        [BindProperty] public double TotalManadatoryAmount { get; set; }
        [BindProperty] public double TotalOptionalAmount { get; set; }
        [BindProperty] public IEnumerable<string> CalculationSteps { get; set; }
        [BindProperty] public IEnumerable<string> Errors { get; set; }
        [BindProperty] public string Id { get; set; }

        private readonly IOfficialFee officialFee;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IOfficialFee officialFee, ILogger<IndexModel> logger)
        {
            this.officialFee = officialFee;
            Errors = new List<string>();
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.Id = id;
            var res = await officialFee.GetVariables(id);
            if (!res.IsSuccessfull)
            {
                TempData["ParseErrors"] = (res as OfficialFeeResultFail).Errors.Distinct().ToList();
                return RedirectToPage("Error");
            }
            Vars = (res as OfficialFeeParseSuccess).RequestedVariables;
            return Page();
        }

        public async Task<IActionResult> OnPostResultAsync(string id, IFormCollection form)
        {
            CollectedVars = new List<IPFValue>();

            // Cycle through all form fields
            foreach (var field in form)
            {
                var CalcVar = Vars.SingleOrDefault(s => s.Name.Equals(field.Key));
                if (CalcVar == null) continue;
                switch (CalcVar)
                {
                    case DslVariableList:
                        CollectedVars.Add(new IPFValueString(CalcVar.Name, field.Value));
                        break;
                    case DslVariableNumber:
                        _ = int.TryParse(field.Value, out var val2);
                        CollectedVars.Add(new IPFValueNumber(CalcVar.Name, val2));
                        break;
                    case DslVariableBoolean:
                        _ = bool.TryParse(field.Value[0], out var val3);
                        CollectedVars.Add(new IPFValueBoolean(CalcVar.Name, val3));
                        break;
                }
            }

            var result = await officialFee.Calculate(id, CollectedVars);

            if (result is OfficialFeeResultFail)
            {
                Errors = (result as OfficialFeeResultFail).Errors;
                // Log calculation failure
                _logger.LogInformation("Fail!");
                foreach (var e in Errors)
                {
                    _logger.LogInformation($"> {e}");
                }
            }
            else
            {
                var result1 = (result as OfficialFeeCalculationSuccess);
                TotalManadatoryAmount = result1.TotalMandatoryAmount;
                TotalOptionalAmount = result1.TotalOptionalAMount;
                CalculationSteps = result1.CalculationSteps;
                // Log computation success
                _logger.LogInformation("Success! Total mandatory amount is [{0}] and the total optional amount is [{1}]", TotalManadatoryAmount, TotalOptionalAmount);
                foreach (var cs in CalculationSteps)
                {
                    _logger.LogInformation($"> {cs}");
                }
            }

            return Page();
        }
    }
}
