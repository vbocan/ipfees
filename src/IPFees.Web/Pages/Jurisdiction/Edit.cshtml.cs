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
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public EditModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            var info = await jurisdictionRepository.GetJurisdictionByName(Id);
            Name = info.Name;
            Description = info.Description;
            SourceCode = info.SourceCode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await jurisdictionRepository.SetJurisdictionSourceCodeAsync(Name, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting jurisdiction source code: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
