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
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("JurisdictionRepository-Add");
            Assert.True(res1.Success);
            var res2 = jur.GetJurisdictions().Any(a => a.Name.Equals("JurisdictionRepository-Add"));
            Assert.True(res2);
        }

        [Fact]
        public async void AddDuplicateJurisdictionTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("JurisdictionRepository-DuP");
            var res2 = await jur.AddJurisdictionAsync("JurisdictionRepository-DuP");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async void SetJurisdictionDescriptionTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("JurisdictionRepository-Set-Description");
            Assert.True(res1.Success);
            var res2 = await jur.SetJurisdictionDescriptionAsync("JurisdictionRepository-Set-Description", "JurisdictionRepository Description");
            Assert.True(res2.Success);
            var mi = jur.GetJurisdictionByName("JurisdictionRepository-Set-Description");
            Assert.Equal("JurisdictionRepository Description", mi.Description);
        }

        [Fact]
        public async void SetJurisdictionSourceCodeTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("JurisdictionRepository-Set-SourceCode");
            Assert.True(res1.Success);
            var res2 = await jur.SetJurisdictionSourceCodeAsync("JurisdictionRepository-Set-SourceCode", "Source Code");
            Assert.True(res2.Success);
            var mi = jur.GetJurisdictionByName("JurisdictionRepository-Set-SourceCode");
            Assert.Equal("Source Code", mi.SourceCode);
        }

        [Fact]
        public async void RemoveJurisdictionsTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("JurisdictionRepository-Del");
            var res2 = await jur.RemoveJurisdictionAsync("JurisdictionRepository-Del");
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }

        [Fact]
        public async void SetJurisdictionReferencedModulesTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("JurisdictionRepository-Set-RefMods");
            Assert.True(res1.Success);
            var res2 = await jur.SetReferencedModules("JurisdictionRepository-Set-RefMods", new string[] { "Mod1", "Mod2" });
            Assert.True(res2.Success);
            var mi = jur.GetJurisdictionByName("JurisdictionRepository-Set-RefMods");
            Assert.Equal(2, mi.ReferencedModules.Length);
        }
    }
}