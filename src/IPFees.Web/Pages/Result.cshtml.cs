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
        public IEnumerable<IPFVariable> Vars { get; set; }
        [BindProperty]
        public int TotalAmount { get; set; }
        [BindProperty]
        public IEnumerable<string> ComputationSteps { get; set; }

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

        public IActionResult OnPostDisplay(IFormCollection form)
        {
            string Code = (string)TempData.Peek("code");
            _calc.Parse(Code);
            Vars = _calc.GetVariables();

            var vars = new List<IPFValue>();

            // Cycle through all form fields
            foreach (var field in form)
            {
                var CalcVar = Vars.SingleOrDefault(s => s.Name.Equals(field.Key));
                if (CalcVar == null) continue;
                switch (CalcVar)
                {
                    case IPFVariableList:
                        var cv1 = (IPFVariableList)CalcVar;
                        vars.Add(new IPFValueString(CalcVar.Name, field.Value));
                        break;
                    case IPFVariableNumber:
                        var cv2 = (IPFVariableNumber)CalcVar;
                        var res2 = int.TryParse(field.Value, out var val2);
                        vars.Add(new IPFValueNumber(CalcVar.Name, val2));
                        break;
                    case IPFVariableBoolean:
                        var cv3 = (IPFVariableBoolean)CalcVar;
                        var res3 = bool.TryParse(field.Value, out var val3);
                        vars.Add(new IPFValueBoolean(CalcVar.Name, val3));
                        break;
                }
            }

            (TotalAmount, ComputationSteps) = _calc.Compute(vars.ToArray());

            return Page();
        }
    }
}
