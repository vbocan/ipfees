using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
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
        private readonly IModuleRepository moduleRepository;

        public DataCollectModel(IDslCalculator IPFCalculator, IModuleRepository moduleRepository, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            this.moduleRepository = moduleRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var RefMod = (IEnumerable<string>)TempData.Peek("modules");
            foreach (var rm in RefMod)
            {
                var module = await moduleRepository.GetModuleByName(rm);
                _calc.Parse(module.SourceCode);
            }

            var Code = (string)TempData.Peek("code");
            _calc.Parse(Code);
            
            Vars = _calc.GetVariables();
            return Page();
        }
    }
}
