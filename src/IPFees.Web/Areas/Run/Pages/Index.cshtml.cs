using IPFees.Core.CurrencyConversion;
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
        [BindProperty] public string[] SelectedJurisdictions { get; set; }

        public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; }
        public IEnumerable<SelectListItem> CurrencyItems { get; set; }
        public IEnumerable<SelectListItem> SelectedJurisdictionItems { get; set; }
        public bool CurrencyExchangeRatesAvailable { get; set; }
        public ResponseStatus CurrencyDataStatus { get; set; }
        
        private const string DefaultCurrency = "EUR";
        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly ICurrencyConverter serd;
        private readonly ILogger<IndexModel> _logger;

        private readonly CurrencySettings currencySettings;

        public IndexModel(IJurisdictionRepository jurisdictionRepository, ICurrencyConverter serd, IOptions<CurrencySettings> currencySettings, ILogger<IndexModel> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.serd = serd;
            this.currencySettings = currencySettings.Value;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Jurisdictions = await jurisdictionRepository.GetJurisdictions();

            SelectedJurisdictionItems = Jurisdictions
                .OrderBy(o => o.Name)
                .Select(s => new SelectListItem($"[{s.Name}] {s.Description}", s.Name, false))
                .ToList();

            CurrencyItems = serd
                .GetCurrencies()
                .Where(w => currencySettings.AllowedCurrencies.Contains(w.Item1))
                .Select(s => new SelectListItem($"[{s.Item1}] {s.Item2}", s.Item1, s.Item1.Equals(DefaultCurrency)));

            CurrencyDataStatus = serd.Response.Status;
            CurrencyExchangeRatesAvailable = serd.Response.Status != ResponseStatus.Invalid;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("DataCollect", new { area = "Run", Id = SelectedJurisdictions, TargetCurrency });
        }
    }

    public record JurisdictionViewModel(Guid Id, string Name, string Description, bool Checked);
}
