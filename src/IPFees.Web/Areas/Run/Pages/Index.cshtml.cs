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
        [BindProperty] public string Id { get; set; }
        [BindProperty] public bool CalculationPending { get; set; } = true;

        [BindProperty] public IList<ItemViewModel> VarItems { get; set; }
        [BindProperty] public IList<IPFValue> CollectedValues { get; set; }

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
            var ParsedVars = (res as OfficialFeeParseSuccess).ParsedVariables;
            VarItems = ParsedVars.Select(pv => new ItemViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostResultAsync(string id)
        {
            CollectedValues = new List<IPFValue>();

            // Cycle through all form fields to discover the type of individual form items
            foreach (var item in VarItems)
            {
                if (item.VarType == typeof(DslVariableList).ToString())
                {
                    CollectedValues.Add(new IPFValueString(item.Name, item.StrValue));
                }
                else if (item.VarType == typeof(DslVariableListMultiple).ToString())
                {
                    CollectedValues.Add(new IPFValueStringList(item.Name, item.ListValue));
                }
                else if (item.VarType == typeof(DslVariableNumber).ToString())
                {
                    //_ = int.TryParse(item.Value, out var val2);
                    CollectedValues.Add(new IPFValueNumber(item.Name, item.DoubleValue));
                }
                else if (item.VarType == typeof(DslVariableBoolean).ToString())
                {
                    //_ = bool.TryParse(item.Value, out var val3);
                    CollectedValues.Add(new IPFValueBoolean(item.Name, item.BoolValue));
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
            }

            return Page();
        }
    }

    public record ItemViewModel(string Name, string VarType, DslVariable Var, string StrValue, string[] ListValue, double DoubleValue, bool BoolValue);
}
