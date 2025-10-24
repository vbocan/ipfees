using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Module.Pages
{    
    public class EditModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public string Description { get; set; } = null!;
        [BindProperty] public bool AutoRun { get; set; }
        [BindProperty] public string SourceCode { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IModuleRepository moduleRepository;

        public EditModel(IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            var info = await moduleRepository.GetModuleById(Id);
            Name = info.Name;
            Description = info.Description;
            AutoRun = info.AutoRun;
            SourceCode = info.SourceCode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            var res = await moduleRepository.SetModuleNameAsync(Id, Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting name: {res.Reason}");
            }
            res = await moduleRepository.SetModuleDescriptionAsync(Id, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            res = await moduleRepository.SetModuleSourceCodeAsync(Id, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res.Reason}");
            }
            res = await moduleRepository.SetModuleAutoRunStatusAsync(Id, AutoRun);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting autorun status: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
