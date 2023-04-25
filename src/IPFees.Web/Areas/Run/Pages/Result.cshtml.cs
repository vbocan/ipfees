using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace IPFees.Web.Areas.Run.Pages
{
    public class ResultModel : PageModel
    {
        [BindProperty] public double TotalMandatoryAmount { get; set; }
        [BindProperty] public double TotalOptionalAmount { get; set; }
        [BindProperty] public IEnumerable<string> CalculationSteps { get; set; }
        [BindProperty] public IEnumerable<(string,string)> Returns { get; set; }
        [BindProperty] public string ComputationError { get; set; }
        [BindProperty] public List<IPFValue> CollectedValues { get; set; }
        [BindProperty] public IList<ParsedVariableViewModel> Vars { get; set; }
        //[BindProperty] public IOfficialFee officialFee { get; set; }
        [BindProperty] public Guid[] SelectedJurisdictions { get; set; }

        private readonly ILogger<ResultModel> _logger;

        public ResultModel(IOfficialFee officialFee, ILogger<ResultModel> logger)
        {
            //this.officialFee = officialFee;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(Guid[] Id)
        {
            SelectedJurisdictions = Id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {            
            CollectedValues = new List<IPFValue>();            

            // Cycle through all form fields to build the collected values list
            foreach (var item in Vars)
            {
                if (item.Type == typeof(DslVariableList).ToString())
                {
                    // A single-selection list return a string
                    CollectedValues.Add(new IPFValueString(item.Name, item.StrValue));
                }
                else if (item.Type == typeof(DslVariableListMultiple).ToString())
                {
                    // A multiple-selection list return a string list
                    CollectedValues.Add(new IPFValueStringList(item.Name, item.ListValue));
                }
                else if (item.Type == typeof(DslVariableNumber).ToString())
                {
                    // A number input returns a double
                    CollectedValues.Add(new IPFValueNumber(item.Name, item.DoubleValue));
                }
                else if (item.Type == typeof(DslVariableBoolean).ToString())
                {
                    // A boolean input returns a boolean
                    CollectedValues.Add(new IPFValueBoolean(item.Name, item.BoolValue));
                }
                else if (item.Type == typeof(DslVariableDate).ToString())
                {
                    // A date input returns a date
                    CollectedValues.Add(new IPFValueDate(item.Name, item.DateValue));
                }
            }

            foreach (var id in SelectedJurisdictions)
            {
                try
                {
                    //var ofr = await officialFee.Calculate(id, CollectedValues);
                    // Log computation success
                    _logger.LogInformation("Success! Total mandatory amount is [{0}] and the total optional amount is [{1}]", TotalMandatoryAmount, TotalOptionalAmount);
                    foreach (var cs in CalculationSteps)
                    {
                        _logger.LogInformation("> {0}", cs);
                    }
                }
                catch (Exception ex)
                {
                    ComputationError = ex.Message;
                    // Log computation error
                    _logger.LogInformation("Failed! Error is {0}.", ex.Message);
                }
            }

            return Page();
        }
    }
}
