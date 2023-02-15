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
        public IList<DynamicField> Fields { get; set; }

        private readonly IIPFCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public DataCollectModel(IIPFCalculator IPFCalculator, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            _logger = logger;
        }

        public void OnGet()
        {
            //var Code = (string)TempData["code"];
            //_calc.Parse(Code);
            //var vars = _calc.GetVariables();
            //var fees = _calc.GetFees();
            var vars = new IPFValue[] {
                new IPFValueString("EntityType", "NormalEntity"),
                new IPFValueString("SituationType", "PreparedISA"),
                new IPFValueNumber("SheetCount", 120),
                new IPFValueNumber("ClaimCount", 7)
            };


            Fields = new List<DynamicField>();

            foreach (var v in vars)
            {
                if (v is IPFVariableList)
                {
                    // Create a combobox
                }
                else if (v is IPFVariableNumber)
                {
                    // Create a number input field
                    //Fields.Add(new DynamicField { Name = v.Name, Type = "text" });
                }
                else if (v is IPFVariableBoolean)
                {
                    // Create a boolean input field
                }
            }
        }
    }

    public class DynamicField
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
