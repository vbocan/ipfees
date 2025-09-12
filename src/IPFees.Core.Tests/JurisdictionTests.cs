using IPFees.Core.Tests.Fixture;

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
        public async Task AddJurisdictionTest()
        {
            var jur = fixture.JurisdictionRepository;
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Add");
            Assert.True(res1.Success);
            var res2 = await jur.GetJurisdictionById(res1.Id);
            Assert.True(res2 != null);
        }

        [Fact]
        public async Task AddDuplicateJurisdictionTest()
        {
            var jur = fixture.JurisdictionRepository;
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-DuP");
            var res2 = await jur.AddJurisdictionAsync("Jurisdiction-DuP");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async Task SetJurisdictionNameTest()
        {
            var jur = fixture.JurisdictionRepository;
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Set-Name");
            Assert.True(res1.Success);
            var res2 = await jur.SetJurisdictionNameAsync(res1.Id, "Jurisdiction Name");
            Assert.True(res2.Success);
            var mi = await jur.GetJurisdictionById(res1.Id);
            Assert.Equal("Jurisdiction Name", mi.Name);
        }

        [Fact]
        public async Task SetJurisdictionDescriptionTest()
        {
            var jur = fixture.JurisdictionRepository;
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Set-Description");
            Assert.True(res1.Success);
            var res2 = await jur.SetJurisdictionDescriptionAsync(res1.Id, "Jurisdiction Description");
            Assert.True(res2.Success);
            var mi = await jur.GetJurisdictionById(res1.Id);
            Assert.Equal("Jurisdiction Description", mi.Description);
        }        

        [Fact]
        public async Task RemoveJurisdictionsTest()
        {
            var jur = fixture.JurisdictionRepository;
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Del");
            var res2 = await jur.RemoveJurisdictionAsync(res1.Id);
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }
    }
}