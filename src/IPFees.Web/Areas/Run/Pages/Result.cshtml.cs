using IPFees.Calculator;
using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using static IPFees.Core.FeeCalculator;

namespace IPFees.Web.Areas.Run.Pages
{
    public class ResultModel : PageModel
    {
        [BindProperty] public string ComputationError { get; set; }
        [BindProperty] public List<IPFValue> CollectedValues { get; set; }
        [BindProperty] public IList<InputViewModel> Inputs { get; set; }
        [BindProperty] public Guid[] SelectedFees { get; set; }
        [BindProperty] public IEnumerable<FeeResult> FeeResults { get; set; }
        private readonly IFeeCalculator officialFee;
        private readonly ILogger<ResultModel> _logger;

        public ResultModel(IFeeCalculator officialFee, ILogger<ResultModel> logger)
        {
            this.officialFee = officialFee;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(Guid[] Id)
        {
            SelectedFees = Id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            FeeResults = officialFee.Calculate(SelectedFees.AsEnumerable(), CollectedValues);
            return Page();
        }
    }
}
