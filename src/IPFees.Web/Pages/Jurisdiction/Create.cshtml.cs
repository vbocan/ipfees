using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class CreateModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IEnumerable<ModuleInfo> AvailableModules { get; set; }
        [BindProperty] public IList<string> ReferencedModules { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly IModuleRepository moduleRepository;

        public CreateModel(IJurisdictionRepository jurisdictionRepository, IModuleRepository moduleRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.moduleRepository = moduleRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            AvailableModules = await moduleRepository.GetModules();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await jurisdictionRepository.AddJurisdictionAsync(Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error creating jurisdiction: {res.Reason}");
            }
            res = await jurisdictionRepository.SetJurisdictionDescriptionAsync(Name, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            string[] RefMod = ReferencedModules.Where(w=>!string.IsNullOrEmpty(w)).ToArray();
            res = await jurisdictionRepository.SetReferencedModules(Name, RefMod.ToArray());
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting referenced modules: {res.Reason}");
            }
            res = await jurisdictionRepository.SetJurisdictionSourceCodeAsync(Name, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
