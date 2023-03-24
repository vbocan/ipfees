using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{
    public class CreateModel : PageModel
    {        
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<ModuleViewModel> ReferencedModules { get; set; }
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
            var Mods = await moduleRepository.GetModules();
            ReferencedModules = Mods.Select(s => new ModuleViewModel(s.Id, s.Name, s.Description, s.LastUpdatedOn, false)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res1 = await jurisdictionRepository.AddJurisdictionAsync(Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error creating jurisdiction: {res1.Reason}");
            }
            var res2 = await jurisdictionRepository.SetJurisdictionDescriptionAsync(res1.Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var RefMod = ReferencedModules.Where(w => w.Checked).Select(s => s.Id).ToList();
            var res3 = await jurisdictionRepository.SetReferencedModules(res1.Id, RefMod);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting referenced modules: {res3.Reason}");
            }
            var res4 = await jurisdictionRepository.SetJurisdictionSourceCodeAsync(res1.Id, SourceCode);
            if (!res4.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res4.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }

    public record ModuleViewModel(Guid Id, string Name, string Description, DateTime LastUpdatedOn, bool Checked);
}
