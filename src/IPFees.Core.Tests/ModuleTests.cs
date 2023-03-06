using IPFees.Core.Tests.Fixture;
using IPFFees.Core;
using IPFFees.Core.Data;

namespace IPFees.Core.Tests
{
    public class ModuleTests : IClassFixture<CoreFixture>
    {
        private CoreFixture fixture;

        public ModuleTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void AddModuleTest()
        {
            var mod = new Module(fixture.DbContext, fixture.DateHelper);
            var res1 = await mod.AddModuleAsync("Module-Add", "Description of Module", "N/A");
            Assert.True(res1.Success);
            var res2 = mod.GetModules().Any(a => a.Name.Equals("Module-Add"));
            Assert.True(res2);
        }

        [Fact]
        public async void AddDuplicateModuleTest()
        {
            var mod = new Module(fixture.DbContext, fixture.DateHelper);
            var res1 = await mod.AddModuleAsync("Module-DuP", "Description of Module", "N/A");
            var res2 = await mod.AddModuleAsync("Module-DuP", "Description of Module", "N/A");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async void EditModuleTest()
        {
            var mod = new Module(fixture.DbContext, fixture.DateHelper);
            var res1 = await mod.AddModuleAsync("Module-Edit", "Description of Module", "N/A");
            Assert.True(res1.Success);
            var res2 = await mod.EditModuleAsync("Module-Edit", "Modified description of Module", "Updated N/A");
            Assert.True(res2.Success);
        }

        [Fact]
        public async void GetModulesTest()
        {
            var mod = new Module(fixture.DbContext, fixture.DateHelper);
            var res1 = await mod.AddModuleAsync("Module1", "Description of Module", "N/A");
            var res2 = await mod.AddModuleAsync("Module2", "Description of Module", "N/A");
            var res3 = await mod.AddModuleAsync("Module3", "Description of Module", "N/A");
            Assert.True(res1.Success);
            Assert.True(res2.Success);
            Assert.True(res3.Success);
            Assert.Equal(3, mod.GetModules().Count());
        }

        [Fact]
        public async void RemoveModulesTest()
        {
            var mod = new Module(fixture.DbContext, fixture.DateHelper);
            var res1 = await mod.AddModuleAsync("Module-Del", "Description of Module", "N/A");
            var res2 = await mod.RemoveModuleAsync("Module-Del");
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }
    }
}