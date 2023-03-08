using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class DataCollectModel : PageModel
    {
        [BindProperty]
        public IEnumerable<DslVariable> Vars { get; set; }

        private readonly IDslCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public DataCollectModel(IDslCalculator IPFCalculator, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            _logger = logger;
        }

        public void OnGet()
        {
            var Code = (string)TempData.Peek("code");
            _calc.Parse(Code);
            Vars = _calc.GetVariables();
        }
    }
}
