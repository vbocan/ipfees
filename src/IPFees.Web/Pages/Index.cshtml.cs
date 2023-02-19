using IPFees.Calculator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Code { get; set; }

        [BindProperty]
        public IEnumerable<string> ParseErrors { get; set; }

        private readonly IIPFCalculator _calc;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IIPFCalculator IPFCalculator, ILogger<IndexModel> logger)
        {
            _calc = IPFCalculator;
            _logger = logger;
        }

        public void OnGet()
        {
            if (Request.Cookies["IPFCode"] != null)
            {
                Code = @Request.Cookies["IPFCode"];
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Code)) return Page();
            Response.Cookies.Append("IPFCode", Code);
            // Log code execution
            _logger.LogInformation("Executing code:");
            foreach (var cl in Code.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                _logger.LogInformation("> {0}", cl);
            }
            // Parse code
            if (!_calc.Parse(Code))
            {
                ParseErrors = _calc.GetErrors();
                return Page();
            }
            TempData["code"] = Code;
            return RedirectToPage("DataCollect");
        }
    }
}