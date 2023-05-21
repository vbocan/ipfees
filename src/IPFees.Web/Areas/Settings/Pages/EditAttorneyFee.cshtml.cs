using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class EditAttorneyFeeModel : PageModel
    {
        [BindProperty] public string AttorneyFeeLevel { get; set; }
        [BindProperty] public int Amount { get; set; }
        [BindProperty] public string Currency { get; set; }

        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IKeyValueRepository keyvalueRepository;

        public EditAttorneyFeeModel(IKeyValueRepository keyvalueRepository)
        {
            this.keyvalueRepository = keyvalueRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            AttorneyFeeLevel = Id;
            (Amount, Currency) = await keyvalueRepository.GetAttorneyFeeAsync(Enum.Parse<JurisdictionAttorneyFeeLevel>(Id));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            var res = await keyvalueRepository.SetAttorneyFeeAsync(Enum.Parse<JurisdictionAttorneyFeeLevel>(Id), Amount, Currency);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting attorney fee: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
