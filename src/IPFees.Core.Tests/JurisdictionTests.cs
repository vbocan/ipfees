using IPFees.Core.Tests.Fixture;
using IPFees.Core;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using IPFees.Core.Enum;

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
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Add");
            Assert.True(res1.Success);
            var res2 = await jur.GetJurisdictionById(res1.Id);
            Assert.True(res2 != null);
        }

        [Fact]
        public async void AddDuplicateJurisdictionTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-DuP");
            var res2 = await jur.AddJurisdictionAsync("Jurisdiction-DuP");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async void SetJurisdictionNameTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Set-Name");
            Assert.True(res1.Success);
            var res2 = await jur.SetJurisdictionNameAsync(res1.Id, "Jurisdiction Name");
            Assert.True(res2.Success);
            var mi = await jur.GetJurisdictionById(res1.Id);
            Assert.Equal("Jurisdiction Name", mi.Name);
        }

        [Fact]
        public async void SetJurisdictionDescriptionTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Set-Description");
            Assert.True(res1.Success);
            var res2 = await jur.SetJurisdictionDescriptionAsync(res1.Id, "Jurisdiction Description");
            Assert.True(res2.Success);
            var mi = await jur.GetJurisdictionById(res1.Id);
            Assert.Equal("Jurisdiction Description", mi.Description);
        }        

        [Fact]
        public async void RemoveJurisdictionsTest()
        {
            var jur = new JurisdictionRepository(fixture.DbContext);
            var res1 = await jur.AddJurisdictionAsync("Jurisdiction-Del");
            var res2 = await jur.RemoveJurisdictionAsync(res1.Id);
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }
    }
}