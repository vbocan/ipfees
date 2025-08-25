using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{    
    public class EditServiceFeeModel : PageModel
    {
        [BindProperty] public string ServiceFeeLevel { get; set; }
        [BindProperty] public decimal Amount { get; set; }
        [BindProperty] public string Currency { get; set; }

        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly ISettingsRepository settingRepository;

        public EditServiceFeeModel(ISettingsRepository settingRepository)
        {
            this.settingRepository = settingRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            ServiceFeeLevel = Id;
            var afi = await settingRepository.GetServiceFeeAsync(Enum.Parse<ServiceFeeLevel>(Id));
            Amount = afi.Amount;
            Currency = afi.Currency;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            var res = await settingRepository.SetServiceFeeAsync(Enum.Parse<ServiceFeeLevel>(Id), Amount, Currency);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting service fee: {res.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
