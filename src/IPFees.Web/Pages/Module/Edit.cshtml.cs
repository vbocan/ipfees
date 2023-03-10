using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages.Module
{
    public class EditModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IModuleRepository moduleRepository;

        public EditModel(IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            var info = await moduleRepository.GetModuleByName(Id);
            Name = info.Name;
            Description = info.Description;
            SourceCode = info.SourceCode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await moduleRepository.SetModuleDescriptionAsync(Name, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting module description: {res.Reason}");
            }
            res = await moduleRepository.SetModuleSourceCodeAsync(Name, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting module source code: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
