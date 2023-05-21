using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Settings.Pages
{
    public class EditCategoryModel : PageModel
    {
        [BindProperty] public string Category { get; set; }
        [BindProperty] public int Weight { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public IList<string> ErrorMessages { get; set; }

        private readonly IKeyValueRepository keyvalueRepository;

        public EditCategoryModel(IKeyValueRepository keyvalueRepository)
        {
            this.keyvalueRepository = keyvalueRepository;
            ErrorMessages = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync(string Id)
        {            
            Category = Id;
            (Weight, Description) = await keyvalueRepository.GetCategoryAsync(Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id)
        {
            var res = await keyvalueRepository.SetCategoryAsync(Id, Weight, Description);
            if (!res.Success)
            {
                ErrorMessages.Add($"Error updating category: {res.Reason}");
            }
            
            if (ErrorMessages.Any()) return Page();
            else return RedirectToPage("Index");
        }
    }
}
