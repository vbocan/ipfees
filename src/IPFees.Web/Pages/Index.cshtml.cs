using IPFees.Calculator;
using IPFees.Web.Areas.Jurisdiction.Pages;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string Code { get; set; }
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; }

        [BindProperty] public IEnumerable<string> ParseErrors { get; set; }

        private readonly IDslCalculator _calc;
        private readonly ILogger<IndexModel> _logger;
        private readonly IModuleRepository moduleRepository;

        public IndexModel(IDslCalculator IPFCalculator, IModuleRepository moduleRepository, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            this.moduleRepository = moduleRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Request.Cookies["code"] != null)
            {
                Code = @Request.Cookies["code"];
            }
            // Prepare view model for referenced modules
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Name, s.Description, s.LastUpdatedOn, false)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Code)) return Page();
            Response.Cookies.Append("code", Code);
            // Log code execution
            _logger.LogInformation("Executing code:");
            foreach (var cl in Code.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                _logger.LogInformation("> {0}", cl);
            }
            // Get referenced modules
            var RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Name).ToList();
            // Parse code
            if (!_calc.Parse(Code))
            {
                ParseErrors = _calc.GetErrors();
                return Page();
            }
            TempData["code"] = Code;
            TempData["modules"] = RefMod;
            return RedirectToPage("DataCollect");
        }
    }
}