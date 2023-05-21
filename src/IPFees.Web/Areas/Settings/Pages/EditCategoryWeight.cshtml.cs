using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class EditCategoryWeightModel : PageModel
    {
        [BindProperty] public string Category { get; set; }
        [BindProperty] public int Weight { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IKeyValueRepository keyvalueRepository;

        public EditCategoryWeightModel(IKeyValueRepository keyvalueRepository)
        {
            this.keyvalueRepository = keyvalueRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {            
            Category = Id;
            Weight = await keyvalueRepository.GetCategoryWeightAsync(Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            var res = await keyvalueRepository.SetCategoryWeightAsync(Id, Weight);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error setting weight: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
