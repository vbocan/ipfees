using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class ReferenceModel : PageModel
    {
        private readonly ILogger<ReferenceModel> _logger;

        public ReferenceModel(ILogger<ReferenceModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}