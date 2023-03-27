using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFees.Web.Areas.Jurisdiction.Pages;
using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class SplitScreenModel : PageModel
    {
        [BindProperty] public string Code { get; set; }
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; }
        [BindProperty] public bool CalculationPending { get; set; } = true;
        [BindProperty] public bool ExecutionPending { get; set; } = true;

        [BindProperty] public IEnumerable<string> ParseErrors { get; set; }

        [BindProperty] public IEnumerable<DslVariable> Vars { get; set; }

        [BindProperty] public double TotalMandatoryAmount { get; set; }
        [BindProperty] public double TotalOptionalAmount { get; set; }
        [BindProperty] public IEnumerable<string> CalculationSteps { get; set; }
        [BindProperty] public string ComputationError { get; set; }
        [BindProperty] public List<IPFValue> CollectedValues { get; set; }

        private readonly IDslCalculator _calc;
        private readonly ILogger<IndexModel> _logger;
        private readonly IModuleRepository moduleRepository;

        public SplitScreenModel(IDslCalculator IPFCalculator, IModuleRepository moduleRepository, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            this.moduleRepository = moduleRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            /// Prepare view model for referenced modules
            var Mods = await moduleRepository.GetModules();
            if (TempData["modules"] != null)
            {
                var RefMod = (IEnumerable<string>)TempData["modules"];
                ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, RefMod.Contains(s.Id.ToString()))).ToList();
            }
            else
            {
                ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, false)).ToList();
            }

            if (Request.Cookies["code"] != null)
            {
                Code = Request.Cookies["code"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostExecuteCodeAsync()
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
            _calc.Parse(Code);

            // Store parsed variables
            Vars = _calc.GetVariables();
            if (RefMod.Any()) TempData["modules"] = RefMod;
            // Prepare view model for referenced modules
            var Mods = moduleRepository.GetModules().Result;
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, RefMod.Contains(s.Name))).ToList();
            ExecutionPending = false;
            return Page();
        }

        public async Task<IActionResult> OnPostResultAsync(IFormCollection form)
        {
            // Prepare view model for referenced modules            
            var RefMod = (IEnumerable<string>)TempData["modules"] ?? Enumerable.Empty<string>();
            foreach (var rm in RefMod)
            {
                var module = await moduleRepository.GetModuleById(Guid.Parse(rm));
                _calc.Parse(module.SourceCode);
            }
            // Parse code
            if (Request.Cookies["code"] != null)
            {
                Code = Request.Cookies["code"];
            }
            _calc.Parse(Code);

            Vars = _calc.GetVariables();

            CollectedValues = new List<IPFValue>();

            // Cycle through all form fields
            foreach (var field in form)
            {
                var CalcVar = Vars.SingleOrDefault(s => s.Name.Equals(field.Key));
                if (CalcVar == null) continue;
                switch (CalcVar)
                {
                    case DslVariableList:
                        CollectedValues.Add(new IPFValueString(CalcVar.Name, field.Value));
                        break;
                    case DslVariableListMultiple:
                        CollectedValues.Add(new IPFValueStringList(CalcVar.Name, field.Value));
                        break;
                    case DslVariableNumber:
                        _ = int.TryParse(field.Value, out var val2);
                        CollectedValues.Add(new IPFValueNumber(CalcVar.Name, val2));
                        break;
                    case DslVariableBoolean:
                        _ = bool.TryParse(field.Value[0], out var val3);
                        CollectedValues.Add(new IPFValueBoolean(CalcVar.Name, val3));
                        break;
                    case DslVariableDate:
                        _ = DateOnly.TryParse(field.Value[0], out var val4);
                        CollectedValues.Add(new IPFValueDate(CalcVar.Name, val4));
                        break;
                }
            }
            // Log variable collection
            _logger.LogInformation("COMPUTATION:");
            foreach (var cv in CollectedValues)
            {
                _logger.LogInformation("> {0}", cv);
            }

            try
            {
                (TotalMandatoryAmount, TotalOptionalAmount, CalculationSteps) = _calc.Compute(CollectedValues);
                // Log computation success
                _logger.LogInformation("Success! Total mandatory amount is [{0}] and the total optional amount is [{1}]", TotalMandatoryAmount, TotalOptionalAmount);
                foreach (var cs in CalculationSteps)
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
            if (RefMod.Any()) TempData["modules"] = RefMod;
            // Prepare view model for referenced modules
            var Mods = moduleRepository.GetModules().Result;
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, RefMod.Contains(s.Name))).ToList();
            CalculationPending = false;
            return Page();
        }
    }
}