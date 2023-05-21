using IPFees.Calculator;
using IPFees.Web.Areas.Jurisdiction.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IPFees.Core.Repository;

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
            var RefMod = (IEnumerable<string>)TempData.Peek("modules") ?? Enumerable.Empty<string>();
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, RefMod.Contains(s.Id.ToString()))).ToList();
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
            var RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Id.ToString()).ToList();
            // Parse module source code
            foreach (var rm in RefMod)
            {
                var Mod = await moduleRepository.GetModuleById(Guid.Parse(rm));
                _calc.Parse(Mod.SourceCode);
            }
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