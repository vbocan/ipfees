using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class GrammarModel : PageModel
    {
        private readonly ILogger<GrammarModel> _logger;

        public GrammarModel(ILogger<GrammarModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}