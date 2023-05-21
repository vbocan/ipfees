using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class EditModel : PageModel
    {
        [BindProperty] public string Category { get; set; }
        [BindProperty] public int Weight { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IKeyValueRepository keyvalueRepository;

        public EditModel(IKeyValueRepository keyvalueRepository)
        {
            this.keyvalueRepository = keyvalueRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {            
            Category = Id;
            Weight = await keyvalueRepository.GetKeyAsync(Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            var res = await keyvalueRepository.SetKeyAsync(Id, Weight);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting weight: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
