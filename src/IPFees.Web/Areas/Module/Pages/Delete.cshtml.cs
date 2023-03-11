using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Module.Pages
{
    public class DeleteModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IModuleRepository moduleRepository;

        public DeleteModel(IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            var info = await moduleRepository.GetModuleByName(Id);
            Name = info.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await moduleRepository.RemoveModuleAsync(Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error deleting module: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
