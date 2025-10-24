using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{    
    public class CreateModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public string Description { get; set; } = null!;
        [BindProperty] public string ServiceFeeLevel { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        public IEnumerable<SelectListItem> ServiceFeeLevelItems { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public CreateModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            ServiceFeeLevelItems = Enum.GetValues<ServiceFeeLevel>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));
            ErrorMessages = new List<string>();
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res1 = await jurisdictionRepository.AddJurisdictionAsync(Name);
            if (!res1.Success)
            {
                ErrorMessages.Add($"Error creating jurisdiction: {res1.Reason}");
            }
            var res2 = await jurisdictionRepository.SetJurisdictionDescriptionAsync(res1.Id, Description);
            if (!res2.Success)
            {
                ErrorMessages.Add($"Error setting description: {res2.Reason}");
            }
            var parsedServiceFeeLevel = (ServiceFeeLevel)Enum.Parse(typeof(ServiceFeeLevel), ServiceFeeLevel);
            var res3 = await jurisdictionRepository.SetJurisdictionServiceFeeLevelAsync(res1.Id, parsedServiceFeeLevel);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res3.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
