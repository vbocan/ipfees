using IPFees.Core.CurrencyConversion;
using IPFees.Core.Enum;
using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace IPFees.Web.Areas.Run.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string TargetCurrency { get; set; }
        [BindProperty] public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        [BindProperty] public IList<JurisdictionViewModel> SelectedJurisdictions { get; set; }

        public IEnumerable<SelectListItem> CurrencyItems { get; set; }
        public bool CurrencyExchangeRatesAvailable { get; set; }
        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly SharedExchangeRateData serd;
        private readonly ILogger<IndexModel> _logger;

        private readonly CurrencySettings currencySettings;

        public IndexModel(IJurisdictionRepository jurisdictionRepository, SharedExchangeRateData serd, IOptions<CurrencySettings> currencySettings, ILogger<IndexModel> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.serd = serd;
            this.currencySettings = currencySettings.Value;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Jurisdictions = await jurisdictionRepository.GetJurisdictions();
            SelectedJurisdictions = Jurisdictions
                .OrderBy(o => o.Name)
                .Select(s => new JurisdictionViewModel(s.Id, s.Name, s.Description, true))
                .ToList();
            CurrencyItems = serd
                .GetCurrencies()
                .Where(w => currencySettings.AllowedCurrencies.Contains(w.Item1))
                .Select(s => new SelectListItem($"{s.Item1} - {s.Item2}", s.Item1));
            CurrencyExchangeRatesAvailable = serd.Response.ServerResponseValid;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var SelJur = await SelectedJurisdictions.ToAsyncEnumerable().Where(w => w.Checked).Select(s => s.Name).ToListAsync();
            return RedirectToPage("DataCollect", new { area = "Run", Id = SelJur, TargetCurrency });
        }
    }

    public record JurisdictionViewModel(Guid Id, string Name, string Description, bool Checked);
}
