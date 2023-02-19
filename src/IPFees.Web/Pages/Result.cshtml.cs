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
        public int TotalAmount { get; set; }
        [BindProperty]
        public IEnumerable<string> ComputationSteps { get; set; }
        [BindProperty]
        public string ComputationError { get; set; }
        [BindProperty]
        public List<IPFValue> CollectedVars { get; set; }

        private readonly IIPFCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public ResultModel(IIPFCalculator IPFCalculator, ILogger<IndexModel> logger)
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

            CollectedVars = new List<IPFValue>();

            // Cycle through all form fields
            foreach (var field in form)
            {
                var CalcVar = ParsedVars.SingleOrDefault(s => s.Name.Equals(field.Key));
                if (CalcVar == null) continue;
                switch (CalcVar)
                {
                    case IPFVariableList:
                        CollectedVars.Add(new IPFValueString(CalcVar.Name, field.Value));
                        break;
                    case IPFVariableNumber:
                        _ = int.TryParse(field.Value, out var val2);
                        CollectedVars.Add(new IPFValueNumber(CalcVar.Name, val2));
                        break;
                    case IPFVariableBoolean:
                        _ = bool.TryParse(field.Value[0], out var val3);
                        CollectedVars.Add(new IPFValueBoolean(CalcVar.Name, val3));
                        break;
                }
            }
            // Log variable collection
            _logger.LogInformation("COMPUTATION:");
            foreach (var cv in CollectedVars)
            {
                _logger.LogInformation("> {0}", cv);
            }

            try
            {
                (TotalAmount, ComputationSteps) = _calc.Compute(CollectedVars.ToArray());
                // Log computation success
                _logger.LogInformation("Success! Total Amount is [{0}].", TotalAmount);
                foreach (var cs in ComputationSteps)
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
