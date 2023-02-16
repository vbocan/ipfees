using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class ResultModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostDisplay(IFormCollection form)
        {
            var formData = new Dictionary<string, string>();

            foreach (var field in form)
            {
                formData.Add(field.Key, field.Value);
            }

            // TODO: Process the form data as needed

            return Page();
        }
    }
}
