using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{
    public class EditModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; }
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
            // Retrieve the jurisdiction by name
            var jur = await jurisdictionRepository.GetJurisdictionByName(Id);
            Name = jur.Name;
            Description = jur.Description;
            SourceCode = jur.SourceCode;
            // Prepare view model for referenced modules
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Name, s.Description, s.LastUpdatedOn, jur.ReferencedModules.Contains(s.Name))).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await jurisdictionRepository.SetJurisdictionDescriptionAsync(Name, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            string[] RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Name).ToArray();
            res = await jurisdictionRepository.SetReferencedModules(Name, RefMod);
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
