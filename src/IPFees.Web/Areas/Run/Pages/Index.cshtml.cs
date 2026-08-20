using IPFees.Core.Model;
using IPFees.Core.Repository;
using IPFees.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using IPFLang.CurrencyConversion;
using System.Text.RegularExpressions;

namespace IPFees.Web.Areas.Run.Pages
{
    public partial class IndexModel : PageModel
    {
        [BindProperty] public string TargetCurrency { get; set; } = null!;
        [BindProperty] public string[] SelectedJurisdictions { get; set; } = null!;

        public IEnumerable<JurisdictionInfo> Jurisdictions { get; set; } = null!;
        public IEnumerable<SelectListItem> CurrencyItems { get; set; } = null!;
        public IEnumerable<SelectListItem> SelectedJurisdictionItems { get; set; } = null!;
        public IEnumerable<JurisdictionChoice> JurisdictionChoices { get; set; } = null!;
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

            // The flag picker shows a short, human name per tile ("United States of America") rather
            // than the full sentence the corpus stores it as ("Entry of national phase in the United
            // States of America"). The corpus isn't consistent about capitalization or the exact
            // wording ("entry" vs "Entry", "phase in" vs "phase entry in"), so this strips a family of
            // prefixes rather than one exact string.
            JurisdictionChoices = Jurisdictions
                .Select(s => new JurisdictionChoice(s.Name, ShortenJurisdictionName(s.Description), s.Description))
                .OrderBy(o => o.ShortName)
                .ToList();

            CurrencyItems = serd
                .GetCurrencies()
                .Where(w => currencySettings.AllowedCurrencies.Contains(w.Item1))
                .Select(s => new SelectListItem($"[{s.Item1}] {s.Item2}", s.Item1, s.Item1.Equals(DefaultCurrency)));

            CurrencyDataStatus = serd.Response.Status;
            CurrencyExchangeRatesAvailable = serd.Response.Status != ResponseStatus.Invalid;
            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("DataCollect", new { area = "Run", Id = SelectedJurisdictions, TargetCurrency });
        }

        private static string ShortenJurisdictionName(string description)
        {
            var shortened = PhaseEntryPrefix().Replace(description, "");
            if (string.IsNullOrEmpty(shortened))
            {
                return description;
            }
            return char.ToUpperInvariant(shortened[0]) + shortened[1..];
        }

        [GeneratedRegex(@"^entry of (national|regional) phase (entry )?in (the )?", RegexOptions.IgnoreCase)]
        private static partial Regex PhaseEntryPrefix();
    }

    public record JurisdictionViewModel(Guid Id, string Name, string Description, bool Checked);
    public record JurisdictionChoice(string Code, string ShortName, string FullDescription);
}
