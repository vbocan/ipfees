using IPFees.Core.Enum;
using IPFees.Core.Repository;
using IPFees.Core.Tests.Fixture;

namespace IPFees.Core.Tests
{
    public class FeeTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public FeeTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void AddFeeTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Add");
            Assert.True(res1.Success);
            var res2 = await jur.GetFeeById(res1.Id);
            Assert.True(res2 != null);
        }

        [Fact]
        public async void AddDuplicateFeeTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-DuP");
            var res2 = await jur.AddFeeAsync("Fee-DuP");
            Assert.True(res1.Success);
            Assert.False(res2.Success);
        }

        [Fact]
        public async void SetFeeNameTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Set-Name");
            Assert.True(res1.Success);
            var res2 = await jur.SetFeeNameAsync(res1.Id, "Fee Name");
            Assert.True(res2.Success);
            var mi = await jur.GetFeeById(res1.Id);
            Assert.Equal("Fee Name", mi.Name);
        }

        [Fact]
        public async void SetFeeJurisdictionNameTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Set-JurisdictionName");
            Assert.True(res1.Success);
            var res2 = await jur.SetFeeJurisdictionNameAsync(res1.Id, "Jurisdiction Name");
            Assert.True(res2.Success);
            var mi = await jur.GetFeeById(res1.Id);
            Assert.Equal("Jurisdiction Name", mi.JurisdictionName);
        }

        [Fact]
        public async void SetFeeCategoryTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Set-Category");
            Assert.True(res1.Success);
            var res2 = await jur.SetFeeCategoryAsync(res1.Id, FeeCategory.OfficialFees);
            Assert.True(res2.Success);
            var mi = await jur.GetFeeById(res1.Id);
            Assert.Equal(FeeCategory.OfficialFees, mi.Category);
        }        

        [Fact]
        public async void SetFeeDescriptionTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Set-Description");
            Assert.True(res1.Success);
            var res2 = await jur.SetFeeDescriptionAsync(res1.Id, "Fee Description");
            Assert.True(res2.Success);
            var mi = await jur.GetFeeById(res1.Id);
            Assert.Equal("Fee Description", mi.Description);
        }

        [Fact]
        public async void SetFeeSourceCodeTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Set-SourceCode");
            Assert.True(res1.Success);
            var res2 = await jur.SetFeeSourceCodeAsync(res1.Id, "Source Code");
            Assert.True(res2.Success);
            var mi = await jur.GetFeeById(res1.Id);
            Assert.Equal("Source Code", mi.SourceCode);
        }

        [Fact]
        public async void RemoveFeesTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Del");
            var res2 = await jur.RemoveFeeAsync(res1.Id);
            Assert.True(res1.Success);
            Assert.True(res2.Success);
        }

        [Fact]
        public async void SetFeeReferencedModulesTest()
        {
            var jur = new FeeRepository(fixture.DbContext);
            var res1 = await jur.AddFeeAsync("Fee-Set-RefMods");
            Assert.True(res1.Success);
            var res2 = await jur.SetReferencedModules(res1.Id, new Guid[] { Guid.NewGuid(), Guid.NewGuid() });
            Assert.True(res2.Success);
            var mi = await jur.GetFeeById(res1.Id);
            Assert.Equal(2, mi.ReferencedModules.Count());
        }
    }
}