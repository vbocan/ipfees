using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{
    public class CreateModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string AttorneyFeeLevel { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        public IEnumerable<SelectListItem> AttorneyFeeLevelItems { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public CreateModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            AttorneyFeeLevelItems = Enum.GetValues<AttorneyFeeLevel>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));
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
            var parsedAttorneyFeeLevel = (AttorneyFeeLevel)Enum.Parse(typeof(AttorneyFeeLevel), AttorneyFeeLevel);
            var res3 = await jurisdictionRepository.SetJurisdictionAttorneyFeeLevelAsync(res1.Id, parsedAttorneyFeeLevel);
            if (!res3.Success)
            {
                ErrorMessages.Add($"Error setting source code: {res3.Reason}");
            }

            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
