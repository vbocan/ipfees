using IPFees.Core.Enum;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPFees.Web.Areas.Jurisdiction.Pages
{    
    public class EditModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = null!;
        [BindProperty] public string Description { get; set; } = null!;
        [BindProperty] public string ServiceFeeLevel { get; set; } = null!;
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        public IEnumerable<SelectListItem> ServiceFeeLevelItems { get; set; }

        private readonly IJurisdictionRepository jurisdictionRepository;

        public EditModel(IJurisdictionRepository jurisdictionRepository)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            ServiceFeeLevelItems = Enum.GetValues<ServiceFeeLevel>().AsEnumerable().Select(s => new SelectListItem(s.ValueAsString(), s.ToString()));
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            var info = await jurisdictionRepository.GetJurisdictionById(Id);
            Name = info.Name;
            Description = info.Description;
            ServiceFeeLevel = info.ServiceFeeLevel.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid Id)
        {
            var res = await jurisdictionRepository.SetJurisdictionNameAsync(Id, Name);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting name: {res.Reason}");
            }
            res = await jurisdictionRepository.SetJurisdictionDescriptionAsync(Id, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting description: {res.Reason}");
            }
            var parsedServiceFeeLevel = (ServiceFeeLevel)Enum.Parse(typeof(ServiceFeeLevel), ServiceFeeLevel);
            res = await jurisdictionRepository.SetJurisdictionServiceFeeLevelAsync(Id, parsedServiceFeeLevel);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting service fee level: {res.Reason}");
            }            
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
