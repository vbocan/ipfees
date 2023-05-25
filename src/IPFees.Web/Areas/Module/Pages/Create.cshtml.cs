using IPFees.Core.Repository;
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
            var res1 = await moduleRepository.AddModuleAsync(Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error creating module: {res1.Reason}");
            }
            var res2 = await moduleRepository.SetModuleDescriptionAsync(res1.Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var res3 = await moduleRepository.SetModuleSourceCodeAsync(res1.Id, SourceCode);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res3.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
