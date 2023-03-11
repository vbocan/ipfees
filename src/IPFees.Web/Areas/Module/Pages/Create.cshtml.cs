using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Module.Pages
{
    public class CreateModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IModuleRepository moduleRepository;

        public CreateModel(IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;
            ErrorMessages = new List<string>();
        }

        public void OnGet()
        {            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await moduleRepository.AddModuleAsync(Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error creating module: {res.Reason}");
            }
            res = await moduleRepository.SetModuleDescriptionAsync(Name, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            res = await moduleRepository.SetModuleSourceCodeAsync(Name, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
