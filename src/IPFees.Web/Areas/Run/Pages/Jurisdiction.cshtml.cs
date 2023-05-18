using IPFees.Calculator;
using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;
using static IPFees.Core.OfficialFee;

namespace IPFees.Web.Areas.Run.Pages
{
    public class JurisdictionModel : PageModel
    {
        [BindProperty] public Guid Id { get; set; }
        [BindProperty] public bool CalculationPending { get; set; } = true;

        [BindProperty] public IList<InputViewModel> Inputs { get; set; }
        [BindProperty] public IList<IPFValue> CollectedValues { get; set; }

        // Calculation results
        [BindProperty] public double TotalMandatoryAmount { get; set; }
        [BindProperty] public double TotalOptionalAmount { get; set; }
        // Calculation steps
        [BindProperty] public IEnumerable<string> CalculationSteps { get; set; }
        // Returns
        [BindProperty] public IEnumerable<(string, string)> Returns { get; set; }

        private readonly IOfficialFee officialFee;
        private readonly ILogger<JurisdictionModel> _logger;

        public JurisdictionModel(IOfficialFee officialFee, ILogger<JurisdictionModel> logger)
        {
            this.officialFee = officialFee;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            this.Id = id;
            var res = officialFee.GetInputs(id);
            if (res is FeeResultFail)
            {
                TempData["Errors"] = (res as FeeResultFail).Errors.ToArray();
                return RedirectToPage("Error");
            }
            Inputs = (res as FeeResultParse).JurisdictionInputs.Select(pv => new InputViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostResultAsync(Guid id)
        {
            CollectedValues = new List<IPFValue>();

            // Cycle through all form fields to build the collected values list
            foreach (var item in Inputs)
            {
                if (item.Type == typeof(DslInputList).ToString())
                {
                    // A single-selection list return a string
                    CollectedValues.Add(new IPFValueString(item.Name, item.StrValue));
                }
                else if (item.Type == typeof(DslInputListMultiple).ToString())
                {
                    // A multiple-selection list return a string list
                    CollectedValues.Add(new IPFValueStringList(item.Name, item.ListValue));
                }
                else if (item.Type == typeof(DslInputNumber).ToString())
                {
                    // A number input returns a double
                    CollectedValues.Add(new IPFValueNumber(item.Name, item.DoubleValue));
                }
                else if (item.Type == typeof(DslInputBoolean).ToString())
                {
                    // A boolean input returns a boolean
                    CollectedValues.Add(new IPFValueBoolean(item.Name, item.BoolValue));
                }
                else if (item.Type == typeof(DslInputDate).ToString())
                {
                    // A date input returns a date
                    CollectedValues.Add(new IPFValueDate(item.Name, item.DateValue));
                }
            }
            // Perform calculation using the values collected from the user
            var result = officialFee.Calculate(id, CollectedValues);

            if (result is FeeResultFail)
            {
                TempData["Errors"] = (result as FeeResultFail).Errors.Distinct().ToList();
                return RedirectToPage("Error");
            }
            else
            {
                var res = (result as FeeResultCalculation);
                TotalMandatoryAmount = res.TotalMandatoryAmount;
                TotalOptionalAmount = res.TotalOptionalAmount;
                CalculationSteps = res.CalculationSteps;
                Returns = res.Returns;
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

    public record InputViewModel(string Name, string Type, DslInput Var, string StrValue, string[] ListValue, double DoubleValue, bool BoolValue, DateOnly DateValue);
    public record FailedJurisdictionViewModel(string JurisdictionName, string JurisdictionDescription);

}
