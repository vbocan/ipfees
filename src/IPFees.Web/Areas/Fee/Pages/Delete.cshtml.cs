using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Fee.Pages
{
    public class DeleteModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IFeeRepository feeRepository;

        public DeleteModel(IFeeRepository feeRepository)
        {
            this.feeRepository = feeRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            var info = await feeRepository.GetJurisdictionById(Id);
            Name = info.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            var res = await feeRepository.RemoveJurisdictionAsync(Id);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error deleting fee: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
