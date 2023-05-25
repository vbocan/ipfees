using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Jurisdiction.Pages
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

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            var info = await jurisdictionRepository.GetJurisdictionById(Id);
            Name = info.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            var res = await jurisdictionRepository.RemoveJurisdictionAsync(Id);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error deleting jurisdiction: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
