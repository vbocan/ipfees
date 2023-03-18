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
        [BindProperty] public string Id { get; set; }
        [BindProperty] public bool CalculationPending { get; set; } = true;
        // Variables found in the source files
        [BindProperty] public IEnumerable<DslVariable> ParsedVars { get; set; }
        // Values collected from the user input
        [BindProperty] public List<IPFValue> CollectedValues { get; set; }
        // Calculation results
        [BindProperty] public double TotalMandatoryAmount { get; set; }
        [BindProperty] public double TotalOptionalAmount { get; set; }
        // Calculation steps
        [BindProperty] public IEnumerable<string> CalculationSteps { get; set; }
        
        private readonly IOfficialFee officialFee;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IOfficialFee officialFee, ILogger<IndexModel> logger)
        {
            this.officialFee = officialFee;            
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            this.Id = id;
            var res = await officialFee.GetVariables(id);
            if (!res.IsSuccessfull)
            {
                TempData["Errors"] = (res as OfficialFeeResultFail).Errors.Distinct().ToList();
                return RedirectToPage("Error");
            }
            ParsedVars = (res as OfficialFeeParseSuccess).ParsedVariables;
            return Page();
        }

        public async Task<IActionResult> OnPostResultAsync(string id, IFormCollection form)
        {
            CollectedValues = new List<IPFValue>();

            // Cycle through all form fields to discover the type of individual form items
            foreach (var field in form)
            {
                var CalcVar = ParsedVars.SingleOrDefault(s => s.Name.Equals(field.Key));
                if (CalcVar == null) continue;
                switch (CalcVar)
                {
                    case DslVariableList:
                        CollectedValues.Add(new IPFValueString(CalcVar.Name, field.Value));
                        break;
                    case DslVariableListMultiple:
                        CollectedValues.Add(new IPFValueStringList(CalcVar.Name, field.Value));
                        break;
                    case DslVariableNumber:
                        _ = int.TryParse(field.Value, out var val2);
                        CollectedValues.Add(new IPFValueNumber(CalcVar.Name, val2));
                        break;
                    case DslVariableBoolean:
                        _ = bool.TryParse(field.Value[0], out var val3);
                        CollectedValues.Add(new IPFValueBoolean(CalcVar.Name, val3));
                        break;
                }
            }
            // Perform calculation using the values collected from the user
            var result = await officialFee.Calculate(id, CollectedValues);

            if (result is OfficialFeeResultFail)
            {
                var Errors = (result as OfficialFeeResultFail).Errors;
                TempData["Errors"] = Errors.ToList();
                // Log calculation failure
                _logger.LogInformation("Fail!");
                foreach (var e in Errors)
                {
                    _logger.LogInformation($"> {e}");
                }
                return RedirectToPage("Error");
            }
            else
            {
                var result1 = (result as OfficialFeeCalculationSuccess);
                TotalMandatoryAmount = result1.TotalMandatoryAmount;
                TotalOptionalAmount = result1.TotalOptionalAMount;
                CalculationSteps = result1.CalculationSteps;
                // Log computation success
                _logger.LogInformation("Success! Total mandatory amount is [{0}] and the total optional amount is [{1}]", TotalMandatoryAmount, TotalOptionalAmount);
                foreach (var cs in CalculationSteps)
                {
                    _logger.LogInformation($"> {cs}");
                }
                CalculationPending = false;
                // TODO: To preserve user input, uncomment the two lines below.
                // TODO: However, the parser will crash because it is invoked twice on the same source code
                //var res = await officialFee.GetVariables(id);                
                //ParsedVars = (res as OfficialFeeParseSuccess).ParsedVariables;                
            }

            return Page();
        }
    }
}
