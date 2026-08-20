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

        /// <summary>
        /// A short excerpt of a real schedule's IPFLang source, shown alongside the calculator so
        /// the language itself is visible without anyone having to go looking for it. Null when no
        /// fee documents are available to draw a snippet from.
        /// </summary>
        public SourceSnippet? SourceSnippet { get; set; }

        private const string DefaultCurrency = "EUR";
        private const int SnippetLineCount = 10;
        private readonly IJurisdictionRepository jurisdictionRepository;
        private readonly IFeeRepository feeRepository;
        private readonly ICurrencyConverter serd;
        private readonly ILogger<IndexModel> _logger;

        private readonly CurrencySettings currencySettings;

        public IndexModel(IJurisdictionRepository jurisdictionRepository, IFeeRepository feeRepository, ICurrencyConverter serd, IOptions<CurrencySettings> currencySettings, ILogger<IndexModel> logger)
        {
            this.jurisdictionRepository = jurisdictionRepository;
            this.feeRepository = feeRepository;
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

            // One fee document per jurisdiction is the canonical schedule (see CorpusSeeder); take
            // the first if more than one is ever registered rather than fail the whole page over it.
            var feesByJurisdiction = (await feeRepository.GetFees())
                .GroupBy(f => f.JurisdictionName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            // The flag picker shows a short, human name per tile ("United States of America") rather
            // than the full sentence the corpus stores it as ("Entry of national phase in the United
            // States of America"). The corpus isn't consistent about capitalization or the exact
            // wording ("entry" vs "Entry", "phase in" vs "phase entry in"), so this strips a family of
            // prefixes rather than one exact string.
            JurisdictionChoices = Jurisdictions
                .Select(s => new JurisdictionChoice(
                    s.Name,
                    ShortenJurisdictionName(s.Description),
                    s.Description,
                    feesByJurisdiction.TryGetValue(s.Name, out var fee) ? fee.Id : null))
                .OrderBy(o => o.ShortName)
                .ToList();

            SourceSnippet = BuildSourceSnippet(feesByJurisdiction);

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

        // US is picked when present because it's the jurisdiction most reviewers recognize on
        // sight; any registered schedule works just as well as a demonstration of the language.
        private static FeeInfo? PickFeatured(IReadOnlyDictionary<string, FeeInfo> feesByJurisdiction) =>
            feesByJurisdiction.TryGetValue("US", out var us)
                ? us
                : feesByJurisdiction.Values.OrderBy(f => f.JurisdictionName, StringComparer.OrdinalIgnoreCase).FirstOrDefault();

        /// <summary>
        /// Every schedule opens with a file-header comment block and a page of input
        /// declarations before the first actual fee calculation, so taking the first N lines
        /// verbatim would show a reviewer nothing but comments. This instead opens on the first
        /// <c>COMPUTE FEE ... ENDCOMPUTE</c> block, which is what the language, and the static
        /// verification pitch beside it, are actually about.
        /// </summary>
        private static SourceSnippet? BuildSourceSnippet(IReadOnlyDictionary<string, FeeInfo> feesByJurisdiction)
        {
            var featured = PickFeatured(feesByJurisdiction);
            if (featured is null) return null;

            var lines = featured.SourceCode.Replace("\r\n", "\n").Split('\n');
            var startIndex = Array.FindIndex(lines, l => l.TrimStart().StartsWith("COMPUTE FEE", StringComparison.OrdinalIgnoreCase));
            if (startIndex < 0) startIndex = 0;

            var block = new List<string>();
            foreach (var raw in lines.Skip(startIndex))
            {
                var line = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(line)) continue;
                block.Add(line);
                if (line.Trim().Equals("ENDCOMPUTE", StringComparison.OrdinalIgnoreCase)) break;
                if (block.Count >= SnippetLineCount) break;
            }

            // There is always more file after the excerpt (declarations before it, or further
            // fees and the VERIFY directives after) unless the block we grabbed is the entire file.
            var truncated = startIndex > 0 || block.Count < lines.Skip(startIndex).Count(l => !string.IsNullOrWhiteSpace(l));
            var text = string.Join('\n', block) + (truncated ? "\n…" : string.Empty);

            return new SourceSnippet(featured.Id, featured.JurisdictionName, text, truncated);
        }
    }

    public record JurisdictionViewModel(Guid Id, string Name, string Description, bool Checked);
    public record JurisdictionChoice(string Code, string ShortName, string FullDescription, Guid? FeeId);

    /// <param name="Text">The excerpt itself, already trimmed to a display-friendly length.</param>
    /// <param name="Truncated">Whether the source has more lines beyond the excerpt.</param>
    public record SourceSnippet(Guid FeeId, string JurisdictionName, string Text, bool Truncated);
}
