using System.Text.RegularExpressions;
using IPFees.Core.CurrencyConversion;
using IPFLang.Corpus;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// The rate table the application falls back on when the rate service cannot be reached.
    ///
    /// Until October 2026 every rate of one or more in that table had lost its decimal point to a
    /// spreadsheet export (USD read as 11.727, JPY as "1.731.823"), and the parser accepted what
    /// it could, so a deployment without a working rate service converted most currencies about
    /// tenfold wrong and silently dropped others.
    /// </summary>
    public class FallbackExchangeRatesTests
    {
        [Fact]
        public void ParseRejectsRatesItCannotReadExactly()
        {
            var (rates, rejected) = FallbackExchangeRates.Parse(new[]
            {
                "Currency;Value", "EUR;1", "CHF;0.925226", "JPY;1.731.823", "USD;1,1225", "usd;1.12", "GBP;", "XAF;655.957",
            });

            Assert.Equal(new Dictionary<string, decimal> { ["EUR"] = 1m, ["CHF"] = 0.925226m, ["XAF"] = 655.957m }, rates);
            Assert.Equal(new[] { "JPY;1.731.823", "USD;1,1225", "usd;1.12", "GBP;" }, rejected);
        }

        [Fact]
        public void ShippedTableIsWholeAndCoversTheCorpus()
        {
            var (rates, rejected) = FallbackExchangeRates.Parse(File.ReadAllLines(ShippedTable()));

            Assert.Empty(rejected);
            Assert.Equal(1m, rates["EUR"]);

            var used = JurisdictionCorpus.Codes.Select(JurisdictionCorpus.GetJurisdiction)
                .Concat(JurisdictionCorpus.BaseNames.Select(JurisdictionCorpus.GetBase))
                .SelectMany(source => Regex.Matches(source, @"RETURN\s+Currency\s+AS\s+'([A-Z]{3})'|<([A-Z]{3})>").Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value))
                .ToHashSet();
            Assert.Empty(used.Where(code => !rates.ContainsKey(code)).Order());
        }

        /// <summary>
        /// Currencies pegged to the euro pin the table's scale: a rate whose decimal point moved
        /// cannot pass.
        /// </summary>
        [Theory]
        [InlineData("BAM", 1.95583, 0.001)]
        [InlineData("XAF", 655.957, 0.001)]
        [InlineData("XOF", 655.957, 0.001)]
        [InlineData("DKK", 7.46038, 0.025)]
        public void ShippedTableMatchesTheEuroPegs(string code, double peg, double tolerance)
        {
            var (rates, _) = FallbackExchangeRates.Parse(File.ReadAllLines(ShippedTable()));
            Assert.InRange((double)rates[code], peg * (1 - tolerance), peg * (1 + tolerance));
        }

        private static string ShippedTable()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            {
                var path = Path.Combine(dir.FullName, "IPFees.Web", "wwwroot", "data", "exchange_rates.csv");
                if (File.Exists(path)) return path;
            }
            throw new FileNotFoundException("exchange_rates.csv not found above " + AppContext.BaseDirectory);
        }
    }
}
