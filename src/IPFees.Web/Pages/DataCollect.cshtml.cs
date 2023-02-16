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
        public IEnumerable<IPFVariable> Vars { get; set; }

        private readonly IIPFCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public DataCollectModel(IIPFCalculator IPFCalculator, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            _logger = logger;
        }

        public void OnGet()
        {
            var Code = (string)TempData["code"];
            _calc.Parse(Code);
            Vars = _calc.GetVariables();
            //var fees = _calc.GetFees();
            //var vars = new IPFValue[] {
            //    new IPFValueString("EntityType", "NormalEntity"),
            //    new IPFValueString("SituationType", "PreparedISA"),
            //    new IPFValueNumber("SheetCount", 120),
            //    new IPFValueNumber("ClaimCount", 7),
            //};            
        }
    }
}
