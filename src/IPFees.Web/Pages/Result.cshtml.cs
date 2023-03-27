using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace IPFees.Web.Pages
{
    public class ResultModel : PageModel
    {
        [BindProperty]
        public double TotalMandatoryAmount { get; set; }
        [BindProperty]
        public double TotalOptionalAmount { get; set; }
        [BindProperty]
        public IEnumerable<string> CalculationSteps { get; set; }
        [BindProperty]
        public string ComputationError { get; set; }
        [BindProperty]
        public List<IPFValue> CollectedValues { get; set; }
        [BindProperty] public IList<ParsedVariableViewModel> Vars { get; set; }

        private readonly IDslCalculator _calc;
        private readonly ILogger<IndexModel> _logger;
        private readonly IModuleRepository moduleRepository;

        public ResultModel(IDslCalculator IPFCalculator, IModuleRepository moduleRepository, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            this.moduleRepository = moduleRepository;
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var RefMod = (IEnumerable<string>)TempData.Peek("modules") ?? Enumerable.Empty<string>();
            foreach (var rm in RefMod)
            {
                var module = await moduleRepository.GetModuleById(Guid.Parse(rm));
                _calc.Parse(module.SourceCode);
            }
            string Code = (string)TempData.Peek("code");
            _calc.Parse(Code);
            var ParsedVars = _calc.GetVariables();

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
            // Log variable collection
            _logger.LogInformation("COMPUTATION:");
            foreach (var cv in CollectedValues)
            {
                _logger.LogInformation("> {0}", cv);
            }

            try
            {
                (TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps) = _calc.Compute(CollectedValues);
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

            return Page();
        }
    }
}
