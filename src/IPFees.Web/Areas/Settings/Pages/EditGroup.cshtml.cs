using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class EditGroupModel : PageModel
    {
        [BindProperty] public string GroupName { get; set; }
        [BindProperty] public string GroupDescription { get; set; }
        [BindProperty] public int GroupIndex { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly ISettingsRepository settingsRepository;

        public EditGroupModel(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {            
            GroupName = Id;
            (GroupDescription, GroupIndex) = await settingsRepository.GetModuleGroupAsync(Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            var res = await settingsRepository.SetModuleGroupAsync(Id, GroupDescription);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error updating group: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
