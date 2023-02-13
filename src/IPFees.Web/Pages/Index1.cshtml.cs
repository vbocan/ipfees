using IPFees.Calculator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class Index1Model : PageModel
    {
        private readonly IIPFCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public Index1Model(IIPFCalculator IPFCalculator, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            _logger = logger;
        }

        public void OnGet()
        {
            var Code = (string)TempData["code"];
            _calc.Parse(Code);
            var vars = _calc.GetVariables();
            var fees = _calc.GetFees();
            // TODO: Display UI based on parsed variables
        }
    }
}
