using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IPFLang.CurrencyConversion;

namespace IPFees.Web.Areas.Currency.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICurrencyConverter serd;
        public IEnumerable<(string, string, decimal?)> Currencies { get; set; } = null!;
        public string ExchangeDataLastUpdatedOn { get; set; } = null!;
        public ResponseStatus DataStatus { get; set; }
        public string DataSourceLabel { get; set; } = null!;
        public string? DataSourceUrl { get; set; }
        public string TimeSinceUpdate { get; set; } = null!;

        public IndexModel(ICurrencyConverter serd)
        {
            this.serd = serd;
        }
        public IActionResult OnGet()
        {
            var response = serd.Response;
            // Invariant culture so the month name is always English, regardless of the host's locale -
            // everything else on the site is English, and this rendered as "20 aug." on a Romanian-locale
            // host without it.
            ExchangeDataLastUpdatedOn = response.LastUpdatedOn.ToString("dd MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
            DataStatus = response.Status;
            (DataSourceLabel, DataSourceUrl) = DescribeSource(response.Status);
            TimeSinceUpdate = DescribeElapsed(DateTime.Now - response.LastUpdatedOn);
            Currencies = GetCurrenciesWithExchangeRate();
            return Page();
        }

        private static (string Label, string? Url) DescribeSource(ResponseStatus status) => status switch
        {
            ResponseStatus.Online => ("exchangerate-api.com", "https://www.exchangerate-api.com"),
            ResponseStatus.Stale => ("Bundled backup rates (exchangerate-api.com unreachable)", null),
            _ => ("Unavailable", null),
        };

        private static string DescribeElapsed(TimeSpan elapsed)
        {
            if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;
            if (elapsed.TotalMinutes < 1) return "just now";
            if (elapsed.TotalMinutes < 60) return Pluralize((int)elapsed.TotalMinutes, "minute");
            if (elapsed.TotalHours < 24) return Pluralize((int)elapsed.TotalHours, "hour");
            return Pluralize((int)elapsed.TotalDays, "day");
        }

        private static string Pluralize(int count, string unit) =>
            $"{count} {unit}{(count == 1 ? "" : "s")} ago";

        private IEnumerable<(string, string, decimal?)> GetCurrenciesWithExchangeRate()
        {
            foreach (var curr in serd.GetCurrencies())
            {
                var Currency = curr.Item1;
                var Description = curr.Item2;
                decimal? ExchangeRate = null;
                try
                {
                    ExchangeRate = Math.Round(serd.Convert(1, Currency, "EUR"), 4);
                }
                catch (Exception) { }
                yield return (Currency, Description, ExchangeRate);
            }
        }
    }
}
