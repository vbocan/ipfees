using IPFees.Core.Tests.Fixture;
using IPFFees.Core;
using IPFFees.Core.Data;

namespace IPFees.Core.Tests
{
    public class JurisdictionTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public JurisdictionTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void AddJurisdictionTest()
        {
            var mod = new Jurisdiction(fixture.DbContext, fixture.Module, fixture.Calculator);
            var res1 = await mod.AddJurisdictionAsync("Jurisdiction-Add");
            Assert.True(res1.Success);
            var res2 = mod.GetJurisdictions().Any(a => a.Name.Equals("Jurisdiction-Add"));
            Assert.True(res2);
        }

        [Fact]
        public async void AddDuplicateJurisdictionTest()
        {
            var mod = new Jurisdiction(fixture.DbContext, fixture.Module, fixture.Calculator);
            var res1 = await mod.AddJurisdictionAsync("Jurisdiction-DuP");
            var res2 = await mod.AddJurisdictionAsync("Jurisdiction-DuP");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async void SetJurisdictionDescriptionTest()
        {
            var mod = new Jurisdiction(fixture.DbContext, fixture.Module, fixture.Calculator);
            var res1 = await mod.AddJurisdictionAsync("Jurisdiction-Set-Description");
            Assert.True(res1.Success);
            var res2 = await mod.SetJurisdictionDescriptionAsync("Jurisdiction-Set-Description", "Jurisdiction Description");
            Assert.True(res2.Success);
            var mi = mod.GetJurisdictionByName("Jurisdiction-Set-Description");
            Assert.Equal("Jurisdiction Description", mi.Description);
        }

        [Fact]
        public async void SetJurisdictionSourceCodeTest()
        {
            var mod = new Jurisdiction(fixture.DbContext, fixture.Module, fixture.Calculator);
            var res1 = await mod.AddJurisdictionAsync("Jurisdiction-Set-SourceCode");
            Assert.True(res1.Success);
            var res2 = await mod.SetJurisdictionSourceCodeAsync("Jurisdiction-Set-SourceCode", "Source Code");
            Assert.True(res2.Success);
            var mi = mod.GetJurisdictionByName("Jurisdiction-Set-SourceCode");
            Assert.Equal("Source Code", mi.SourceCode);
        }

        [Fact]
        public async void RemoveJurisdictionsTest()
        {
            var mod = new Jurisdiction(fixture.DbContext, fixture.Module, fixture.Calculator);
            var res1 = await mod.AddJurisdictionAsync("Jurisdiction-Del");
            var res2 = await mod.RemoveJurisdictionAsync("Jurisdiction-Del");
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }

        [Fact]
        public async void SetJurisdictionReferencedModulesTest()
        {
            var mod = new Jurisdiction(fixture.DbContext, fixture.Module, fixture.Calculator);
            var res1 = await mod.AddJurisdictionAsync("Jurisdiction-Set-RefMods");
            Assert.True(res1.Success);
            var res2 = await mod.SetReferencedModules("Jurisdiction-Set-RefMods", new string[] { "Mod1", "Mod2" });
            Assert.True(res2.Success);
            var mi = mod.GetJurisdictionByName("Jurisdiction-Set-RefMods");
            Assert.Equal(2, mi.ReferencedModules.Count());
        }
    }
}