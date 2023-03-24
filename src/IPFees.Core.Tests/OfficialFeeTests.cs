using IPFees.Calculator;
using IPFees.Core.Tests.Fixture;
using IPFees.Evaluator;
using IPFees.Parser;
using IPFFees.Core;
using IPFFees.Core.Data;
using static IPFees.Core.OfficialFee;

namespace IPFees.Core.Tests
{
    public class OfficialFeeTests : IClassFixture<OfficialFeeFixture>
    {
        private readonly OfficialFeeFixture fixture;

        public OfficialFeeTests(OfficialFeeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void OfficialFeeCalculationTest()
        {
            // First let's create a module containing some source code that will later be referenced by the jurisdiction
            var mod = fixture.ModuleRepository;
            var res2 = await mod.AddModuleAsync("Mod1");
            Assert.True(res2.Success);
            string modSourceCode =
            """
            COMPUTE FEE ModuleFee
            YIELD 100            
            ENDCOMPUTE
            """;
            var res3 = await mod.SetModuleSourceCodeAsync(res2.Id, modSourceCode);
            Assert.True(res3.Success);
            // Now let's create a jurisdiction and the associated module
            var jur = fixture.JurisdictionRepository;
            var res4 = await jur.AddJurisdictionAsync("Jur1");
            Assert.True(res4.Success);
            string jurSourceCode =
            """
            COMPUTE FEE FirstFee
            YIELD 320
            YIELD 80
            ENDCOMPUTE
            """;
            var res5 = await jur.SetJurisdictionSourceCodeAsync(res4.Id, jurSourceCode);
            Assert.True(res5.Success);
            var res6 = await jur.SetReferencedModules(res4.Id, new Guid[] { res2.Id });
            Assert.True(res6.Success);
            var parser = new DslParser();            
            IDslCalculator calc = new DslCalculator(parser);
            OfficialFee of = new(jur, mod, calc);
            var res7 = await of.Calculate(res4.Id, new List<IPFValue> { });
            Assert.True(res7.IsSuccessfull);
            Assert.IsType<OfficialFeeCalculationSuccess>(res7);
            var res = (OfficialFeeCalculationSuccess)res7;
            Assert.Equal(500, res.TotalMandatoryAmount);
        }

    }
}