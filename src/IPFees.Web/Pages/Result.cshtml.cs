using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        private readonly IDslCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public ResultModel(IDslCalculator IPFCalculator, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(IFormCollection form)
        {
            string Code = (string)TempData.Peek("code");
            _calc.Parse(Code);
            var ParsedVars = _calc.GetVariables();

            CollectedValues = new List<IPFValue>();

            // Cycle through all form fields
            foreach (var field in form)
            {
                var CalcVar = ParsedVars.SingleOrDefault(s => s.Name.Equals(field.Key));
                if (CalcVar == null) continue;
                switch (CalcVar)
                {
                    case DslVariableList:
                        CollectedValues.Add(new IPFValueString(CalcVar.Name, field.Value));
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
