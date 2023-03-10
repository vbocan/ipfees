using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class EditModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IEnumerable<ModuleInfo> AvailableModules { get; set; }
        [BindProperty] public IList<string> ReferencedModules { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly IModuleRepository moduleRepository;

        public EditModel(IJurisdictionRepository jurisdictionRepository, IModuleRepository moduleRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.moduleRepository = moduleRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            AvailableModules = await moduleRepository.GetModules();
            var info = await jurisdictionRepository.GetJurisdictionByName(Id);
            Name = info.Name;
            Description = info.Description;
            SourceCode = info.SourceCode;
            ReferencedModules = info.ReferencedModules;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await jurisdictionRepository.SetJurisdictionDescriptionAsync(Name, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            string[] RefMod = ReferencedModules.Where(w => !string.IsNullOrEmpty(w)).ToArray();
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
