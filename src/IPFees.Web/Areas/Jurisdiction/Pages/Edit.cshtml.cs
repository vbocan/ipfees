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

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            // Retrieve the jurisdiction by name
            var jur = await jurisdictionRepository.GetJurisdictionById(Id);
            Name = jur.Name;
            Description = jur.Description;
            SourceCode = jur.SourceCode;
            // Prepare view model for referenced modules
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, jur.ReferencedModules.Contains(s.Id))).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            var res1 = await jurisdictionRepository.SetJurisdictionNameAsync(Id, Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error setting name: {res1.Reason}");
            }
            var res2 = await jurisdictionRepository.SetJurisdictionDescriptionAsync(Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Id).ToList();
            var res3 = await jurisdictionRepository.SetReferencedModules(Id, RefMod);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting referenced modules: {res3.Reason}");
            }
            var res4 = await jurisdictionRepository.SetJurisdictionSourceCodeAsync(Id, SourceCode);
            if (!res4.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res4.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
