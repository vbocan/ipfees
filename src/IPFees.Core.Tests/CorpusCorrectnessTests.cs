using IPFees.Core.FeeCalculation;
using IPFLang.Composition;
using IPFLang.Corpus;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;
using IPFLang.Versioning;

namespace IPFees.Core.Tests
{
    /// <summary>
    /// Properties of the reference corpus that IPFees carries, pinned so that a later edit to a
    /// schedule cannot quietly undo them.
    /// </summary>
    public class CorpusCorrectnessTests
    {
        private static ParsedScript Compose(string code)
        {
            var parser = new DslParser();
            var registry = new JurisdictionRegistry();
            string? parent = null, leaf = null;
            foreach (var (id, source) in JurisdictionCorpus.GetCompositionChain(code))
            {
                var (parsed, errors) = parser.Parse(source, returnParsedScript: true);
                Assert.True(parsed is not null, $"{id}: {string.Join(" | ", errors)}");
                registry.Register(new Jurisdiction(id, id, parsed!, parent));
                parent = id;
                leaf = id;
            }
            return new JurisdictionComposer(registry).Compose(leaf!).Script;
        }

        private static decimal FeeAmount(string code, string fee, IEnumerable<IPFValue> overrides)
        {
            var script = Compose(code);
            var calculator = new DslCalculator(new DslParser());
            calculator.LoadParsedScript(script);
            var values = Defaults(script.Inputs).ToDictionary(v => v.Name);
            foreach (var o in overrides) values[o.Name] = o;
            return calculator.ComputeWithProvenance(values.Values.ToList()).FeeBreakdown.Single(b => b.FeeName == fee).Amount;
        }

        /// <summary>
        /// Every VERIFY COMPLETE in the corpus holds. The last three to do so were the Chinese
        /// fees guarded on the receiving office being a different office: CNIPA waives the filing
        /// fee and the application surcharges when it was the receiving office, and the schedule
        /// now says so.
        /// </summary>
        [Fact]
        public void EveryCompletenessDirectiveHolds()
        {
            var verifier = new ScheduleVerifier();
            var failing = new List<string>();
            foreach (var code in JurisdictionCorpus.Codes)
            {
                var script = Compose(code);
                var result = verifier.Verify(script.Inputs, script.Fees, script.Verifications);
                failing.AddRange(result.CompletenessFailures.Select(f => $"{code}/{f.FeeName}"));
            }

            Assert.Empty(failing);
        }

        [Fact]
        public void TranslationIsPricedInEurEverywhere()
        {
            foreach (var code in JurisdictionCorpus.Codes)
            {
                var script = Compose(code);
                var currency = script.Returns.First(r => r.Symbol == "Currency").Text;
                var fee = script.Fees.Single(f => f.Name == "TranslationFee");
                Assert.Equal("EUR", FeeCurrency.Resolve(fee, script.Inputs, currency));
            }
        }

        /// <summary>The USPTO charges 420 USD for each 50 sheets or part thereof above 100.</summary>
        [Theory]
        [InlineData(100, 0)]
        [InlineData(101, 420)]
        [InlineData(150, 420)]
        [InlineData(151, 840)]
        public void PerBlockChargesRoundUp(int sheets, decimal expected)
        {
            var amount = FeeAmount("US", "OFF_SheetFee", new IPFValue[]
            {
                new IPFValueNumber("SheetCount", sheets),
                new IPFValueString("EntitySize", "Entity_Large"),
            });
            Assert.Equal(expected, amount);
        }

        /// <summary>
        /// The EPO waives the supplementary search fee when it was the international searching
        /// authority, and charges it in full when the authority is not one of those listed for
        /// the reduction.
        /// </summary>
        [Theory]
        [InlineData("ISA_EPO", 0)]
        [InlineData("ISA_AT", 275)]
        [InlineData("ISA_NONE", 1460)]
        public void EuropeanSupplementarySearchFee(string isa, decimal expected)
        {
            Assert.Equal(expected, FeeAmount("EP", "OFF_SearchFee", new IPFValue[] { new IPFValueString("ISA", isa) }));
        }

        private static IEnumerable<IPFValue> Defaults(IEnumerable<DslInput> inputs)
        {
            foreach (var input in inputs.DistinctBy(i => i.Name))
            {
                switch (input)
                {
                    case DslInputNumber n: yield return new IPFValueNumber(input.Name, n.DefaultValue); break;
                    case DslInputBoolean b: yield return new IPFValueBoolean(input.Name, b.DefaultValue); break;
                    case DslInputList l: yield return new IPFValueString(input.Name, l.DefaultSymbol); break;
                    case DslInputListMultiple m: yield return new IPFValueStringList(input.Name, m.DefaultSymbols.ToList()); break;
                    case DslInputDate d: yield return new IPFValueDate(input.Name, d.DefaultValue); break;
                    case DslInputAmount a: yield return new IPFValueNumber(input.Name, a.DefaultValue); break;
                }
            }
        }
    }
}
