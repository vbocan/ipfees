using IPFFees.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages.Jurisdiction
{
    public class CreateModel : PageModel
    {
        [BindProperty] public string Name { get; set; }        
        [BindProperty] public string SourceCode { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public CreateModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            ErrorMessages = new List<string>();
        }

        public void OnGet()
        {            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await jurisdictionRepository.AddJurisdictionAsync(Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error creating jurisdiction: {res.Reason}");
            }
            res = await jurisdictionRepository.SetJurisdictionSourceCodeAsync(Name, SourceCode);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting jurisdiction source code: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
