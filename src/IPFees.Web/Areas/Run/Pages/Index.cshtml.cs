using IPFees.Core.Model;
using IPFees.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPFees.Web.Areas.Run.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public IEnumerable<FeeInfo> Fees { get; set; }
        [BindProperty] public IList<FeeViewModel> SelectedFees { get; set; }

        private readonly IFeeRepository feeRepository;        
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IFeeRepository feeRepository, ILogger<IndexModel> logger)
        {
            this.feeRepository = feeRepository;            
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Fees = await feeRepository.GetFees();
            SelectedFees = Fees.OrderBy(o => o.Name).Select(s => new FeeViewModel(s.Id, s.Name, s.Description, true)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var SelJur = await SelectedFees.ToAsyncEnumerable().Where(w => w.Checked).Select(s => s.Id).ToListAsync();
            return RedirectToPage("DataCollect", new { area = "Run", Id = SelJur });
        }
    }

    public record FeeViewModel(Guid Id, string Name, string Description, bool Checked);
}
