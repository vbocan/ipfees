using IPFLang.Composition;
using IPFLang.Corpus;
using IPFLang.Engine;
using IPFLang.Evaluator;
using IPFLang.Parser;

namespace IPFLang.Engine.Tests
{
    /// <summary>
    /// Corrections to the corpus that change an amount, each pinned at the inputs it concerns. The
    /// amounts are those of the corpus baseline (January 2024); the corrections fix how a fee is
    /// charged, not what it costs.
    /// </summary>
    public class CorpusCorrectionsTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void UsMultipleDependentClaimFeeIsChargedOncePerApplication(int claims)
        {
            Assert.Equal(860m, Fee("US", "OFF_MultipleDependentClaimFee", ("MultipleDependentClaimCount", claims)));
            Assert.Equal(172m, Fee("US", "OFF_MultipleDependentClaimFee", ("MultipleDependentClaimCount", claims), ("EntitySize", "Entity_Micro")));
            Assert.Equal(0m, Fee("US", "OFF_MultipleDependentClaimFee", ("MultipleDependentClaimCount", 0)));
        }

        [Fact]
        public void EpExaminationFeeIsReducedBy75PercentWhenTheEpoDrewUpTheIper()
        {
            Assert.Equal(2055m * 0.25m, Fee("EP", "OFF_ExaminationFee", ("IPRP", "IPRP_EPO"), ("ISA", "ISA_EPO")));
            Assert.Equal(2055m, Fee("EP", "OFF_ExaminationFee", ("IPRP", "IPRP_NONE"), ("ISA", "ISA_EPO")));
        }

        [Fact]
        public void CnFilingFeeAndSurchargesAreWaivedWhenCnipaWasTheReceivingOffice()
        {
            var waived = new (string, object)[] { ("RO_NP", true), ("ClaimCount", 20), ("SheetCount", 40) };
            Assert.Equal(0m, Fee("CN", "OFF_BasicNationalFee", waived));
            Assert.Equal(0m, Fee("CN", "OFF_ClaimFee", waived));
            Assert.Equal(0m, Fee("CN", "OFF_SheetFee", waived));

            var charged = new (string, object)[] { ("RO_NP", false), ("ClaimCount", 20), ("SheetCount", 40) };
            Assert.Equal(900m, Fee("CN", "OFF_BasicNationalFee", charged));
            Assert.Equal(1500m, Fee("CN", "OFF_ClaimFee", charged));
            Assert.Equal(500m, Fee("CN", "OFF_SheetFee", charged));
        }

        [Fact]
        public void CnExaminationFeeIsWaivedWhenCnipaDrewUpTheSearchOrThePreliminaryReport()
        {
            Assert.Equal(0m, Fee("CN", "OFF_ExaminationFee", ("ISA", "ISA_CN")));
            Assert.Equal(0m, Fee("CN", "OFF_ExaminationFee", ("ISA", "ISA_EPO"), ("IPRP", "IPRP_CN")));
            Assert.Equal(2000m, Fee("CN", "OFF_ExaminationFee", ("ISA", "ISA_EPO")));
            Assert.Equal(2500m, Fee("CN", "OFF_ExaminationFee", ("ISA", "ISA_US")));
        }

        [Fact]
        public void BrChargesNoAnnuityBeforeTheThirdYear()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            Assert.Equal(0m, Fee("BR", "OFF_Renewal", ("ApplicationDate", today)));
            Assert.Equal(0m, Fee("BR", "OFF_Renewal", ("ApplicationDate", today.AddMonths(-18))));
            Assert.Equal(780m, Fee("BR", "OFF_Renewal", ("ApplicationDate", today.AddMonths(-30))));
        }

        [Theory]
        [InlineData(30, 0)]
        [InlineData(31, 36)]
        [InlineData(35, 180)]
        public void PhChargesPerSheetInExcessOfThirty(int sheets, int expected)
        {
            Assert.Equal(expected, Fee("PH", "OFF_SheetFee", ("SheetCount", sheets), ("EntitySize", "Entity_Large")));
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(15, 120000)]
        [InlineData(20, 120000)]
        [InlineData(25, 300000)]
        [InlineData(35, 600000)]
        [InlineData(40, 600000)]
        [InlineData(41, 680000)]
        [InlineData(50, 680000)]
        [InlineData(51, 760000)]
        public void OaAcceptanceFeeIsAFlatFeePerBandAndPerStartedSetOfTenAbove40(int sheets, int expected)
        {
            Assert.Equal(expected, Fee("OA", "Off_AcceptanceFee", ("SheetCount", sheets)));
        }

        private static decimal Fee(string code, string feeName, params (string Name, object Value)[] overrides)
        {
            var parser = new DslParser();
            var registry = new JurisdictionRegistry();
            string? parent = null, leaf = null;
            foreach (var (id, source) in JurisdictionCorpus.GetCompositionChain(code))
            {
                var (parsed, errors) = parser.Parse(source, returnParsedScript: true);
                Assert.True(parsed is not null, $"{code}: '{id}' failed to parse: {string.Join(" | ", errors)}");
                registry.Register(new Jurisdiction(id, id, parsed!, parent));
                parent = id;
                leaf = id;
            }

            var calculator = new DslCalculator(new DslParser());
            calculator.LoadParsedScript(new JurisdictionComposer(registry).Compose(leaf!).Script);

            var values = new List<IPFValue>();
            foreach (var input in calculator.GetInputs())
            {
                var given = overrides.Where(o => o.Name == input.Name).Select(o => o.Value).FirstOrDefault();
                values.Add(input switch
                {
                    DslInputNumber n => new IPFValueNumber(input.Name, given is int i ? i : n.DefaultValue),
                    DslInputBoolean b => new IPFValueBoolean(input.Name, given is bool v ? v : b.DefaultValue),
                    DslInputList l => new IPFValueString(input.Name, given as string ?? l.DefaultSymbol),
                    DslInputListMultiple m => new IPFValueStringList(input.Name, m.DefaultSymbols.ToList()),
                    DslInputDate d => new IPFValueDate(input.Name, given is DateOnly date ? date : d.DefaultValue),
                    DslInputAmount a => new IPFValueNumber(input.Name, a.DefaultValue),
                    _ => throw new InvalidOperationException($"Unexpected input kind {input.GetType().Name}"),
                });
            }

            var provenance = calculator.ComputeWithProvenance(values);
            var fee = provenance.FeeProvenances.SingleOrDefault(f => f.FeeName == feeName);
            Assert.True(fee is not null, $"{code} computes no fee '{feeName}'");
            return fee!.TotalAmount;
        }
    }
}
