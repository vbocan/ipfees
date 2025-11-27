using IPFees.Core.FeeCalculation;
using IPFees.Evaluator;
using IPFees.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Run.Pages
{
    public class FeeModel : PageModel
    {
        [BindProperty] public Guid Id { get; set; }
        [BindProperty] public bool CalculationPending { get; set; } = true;

        [BindProperty] public IList<InputViewModel> Inputs { get; set; } = null!;
        [BindProperty] public IList<IPFValue> CollectedValues { get; set; } = null!;

        // Calculation results
        [BindProperty] public decimal TotalMandatoryAmount { get; set; }
        [BindProperty] public decimal TotalOptionalAmount { get; set; }
        // Calculation steps
        [BindProperty] public IEnumerable<string> CalculationSteps { get; set; } = null!;
        // Returns
        [BindProperty] public IEnumerable<(string, string)> Returns { get; set; } = null!;

        private readonly IFeeCalculator officialFee;
        private readonly ILogger<FeeModel> _logger;

        public FeeModel(IFeeCalculator officialFee, ILogger<FeeModel> logger)
        {
            this.officialFee = officialFee;
            _logger = logger;
        }

        public IActionResult OnGet(Guid id)
        {
            this.Id = id;
            var res = officialFee.GetInputs(id);
            if (res is FeeResultFail)
            {
                TempData["Errors"] = ((FeeResultFail)res).Errors.ToArray();
                return RedirectToPage("Error");
            }
            Inputs = ((FeeResultParse)res).FeeInputs.Select(pv => new InputViewModel(pv.Group, pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false, DateOnly.MinValue)).ToList();
            return Page();
        }

        public IActionResult OnPostResult(Guid id)
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
                    CollectedValues.Add(new IPFValueNumber(item.Name, item.DecimalValue));
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
                TempData["Errors"] = ((FeeResultFail)result).Errors.Distinct().ToList();
                return RedirectToPage("Error");
            }
            else
            {
                var res = (FeeResultCalculation)result;
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

    public record InputViewModel(string Group, string Name, string Type, DslInput Var, string StrValue, string[] ListValue, decimal DecimalValue, bool BoolValue, DateOnly DateValue);    
    public record FailedFeeViewModel(string FeeName, string FeeDescription);

}
