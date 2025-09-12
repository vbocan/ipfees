using IPFees.Core.Tests.Fixture;

namespace IPFees.Core.Tests
{
    public class ModuleTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public ModuleTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task AddModuleTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-Add");
            Assert.True(res1.Success);
            var res2 = await mod.GetModuleById(res1.Id);
            Assert.True(res2 != null);
        }

        [Fact]
        public async Task AddDuplicateModuleTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-DuP");
            var res2 = await mod.AddModuleAsync("Module-DuP");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async Task SetModuleNameTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-Set-Name");
            Assert.True(res1.Success);
            var res2 = await mod.SetModuleNameAsync(res1.Id, "Module Name");
            Assert.True(res2.Success);
            var mi = await mod.GetModuleById(res1.Id);
            Assert.Equal("Module Name", mi.Name);
        }

        [Fact]
        public async Task SetModuleDescriptionTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-Set-Description");
            Assert.True(res1.Success);
            var res2 = await mod.SetModuleDescriptionAsync(res1.Id, "Module Description");
            Assert.True(res2.Success);
            var mi = await mod.GetModuleById(res1.Id);
            Assert.Equal("Module Description", mi.Description);
        }

        [Fact]
        public async Task SetModuleAutoRunStatusTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-Set-AutoRun");
            Assert.True(res1.Success);
            var res2 = await mod.SetModuleAutoRunStatusAsync(res1.Id, true);
            Assert.True(res2.Success);
            var mi = await mod.GetModuleById(res1.Id);
            Assert.True(mi.AutoRun);
        }

        [Fact]
        public async Task SetModuleSourceCodeTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-Set-SourceCode");
            Assert.True(res1.Success);
            var res2 = await mod.SetModuleSourceCodeAsync(res1.Id, "Source Code");
            Assert.True(res2.Success);
            var mi = await mod.GetModuleById(res1.Id);
            Assert.Equal("Source Code", mi.SourceCode);
        }

        [Fact]
        public async Task RemoveModulesTest()
        {
            var mod = fixture.ModuleRepository;
            var res1 = await mod.AddModuleAsync("Module-Del");
            var res2 = await mod.RemoveModuleAsync(res1.Id);
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }
    }
}