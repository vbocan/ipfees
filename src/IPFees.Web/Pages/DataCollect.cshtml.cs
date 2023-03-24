using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Web.Areas.Run.Pages;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static IPFees.Core.OfficialFee;

namespace IPFees.Web.Pages
{
    public class DataCollectModel : PageModel
    {        
        [BindProperty] public IList<ParsedVariableViewModel> Vars { get; set; }

        private readonly IDslCalculator _calc;
        private readonly ILogger<IndexModel> _logger;
        private readonly IModuleRepository moduleRepository;

        public DataCollectModel(IDslCalculator IPFCalculator, IModuleRepository moduleRepository, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            this.moduleRepository = moduleRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var RefMod = (IEnumerable<string>)TempData.Peek("modules") ?? Enumerable.Empty<string>();
            foreach (var rm in RefMod)
            {
                var module = await moduleRepository.GetModuleById(Guid.Parse(rm));
                _calc.Parse(module.SourceCode);
            }

            var Code = (string)TempData.Peek("code");
            _calc.Parse(Code);

            Vars = _calc.GetVariables().Select(pv => new ParsedVariableViewModel(pv.Name, pv.GetType().ToString(), pv, string.Empty, Array.Empty<string>(), 0, false)).ToList();
            return Page();
        }
    }

    public record ParsedVariableViewModel(string Name, string Type, DslVariable Var, string StrValue, string[] ListValue, double DoubleValue, bool BoolValue);
}
