using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class DeleteModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public DeleteModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            var info = await jurisdictionRepository.GetJurisdictionByName(Id);
            Name = info.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await jurisdictionRepository.RemoveJurisdictionAsync(Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error deleting jurisdiction: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
