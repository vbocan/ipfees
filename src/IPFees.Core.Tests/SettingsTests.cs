using IPFees.Core.Tests.Fixture;
using IPFees.Core;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using IPFees.Core.Enum;

namespace IPFees.Core.Tests
{
    public class SettingsTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public SettingsTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void SetGetAttorneyFeeTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level1, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await sr.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level1);
            Assert.Equal(150, res2.Amount);
            Assert.Equal("USD", res2.Currency);
        }

        [Fact]
        public async void SetGetAttorneyFeeConsecutiveTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await sr.SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2, 200, "EUR");
            Assert.True(res2.Success);
            var res3 = await sr.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2);
            Assert.Equal(200, res3.Amount);
            Assert.Equal("EUR", res3.Currency);
        }

        [Fact]
        public async void SetGetAttorneyFeeInvalidTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level3);
            Assert.Equal(0, res1.Amount);
            Assert.Equal(string.Empty, res1.Currency);
        }

        [Fact]
        public async void SetGetCategoryTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.SetModuleGroupAsync("Group #1", "Description");
            Assert.True(res1.Success);
            var res2 = await sr.GetModuleGroupAsync("Group #1");            
            Assert.Equal("Description", res2.GroupDescription);
            Assert.Equal(1, res2.GroupIndex);
        }

        [Fact]
        public async void SetGetCategoryConsecutiveTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.SetModuleGroupAsync("Group #2", "Description");
            Assert.True(res1.Success);
            var res2 = await sr.SetModuleGroupAsync("Group #2", "Altered Description");
            Assert.True(res2.Success);
            var res3 = await sr.GetModuleGroupAsync("Group #2");
            Assert.Equal("Altered Description", res3.GroupDescription);
            Assert.Equal(1, res3.GroupIndex);
        }

        [Fact]
        public async void SetGetCategoryInvalidTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.GetModuleGroupAsync("Group #3");
            Assert.Equal(string.Empty, res1.GroupDescription);
            Assert.Equal(-1, res1.GroupIndex);
        }

        [Fact]
        public async void MoveDownCategoryTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            // Create groups
            var res1 = await sr.SetModuleGroupAsync("Group #4", "Description");
            Assert.True(res1.Success);
            var res2 = await sr.SetModuleGroupAsync("Group #5", "Description");
            Assert.True(res2.Success);
            // Check group positions
            var res3 = await sr.GetModuleGroupAsync("Group #4");            
            Assert.Equal(1, res3.GroupIndex);
            var res4 = await sr.GetModuleGroupAsync("Group #5");
            Assert.Equal(2, res4.GroupIndex);
            // Move the first group one position down
            await sr.MoveModuleGroupDownAsync("Group #4");
            // Recheck group positions
            var res5 = await sr.GetModuleGroupAsync("Group #4");
            Assert.Equal(2, res5.GroupIndex);
            var res6 = await sr.GetModuleGroupAsync("Group #5");
            Assert.Equal(1, res6.GroupIndex);
        }

        [Fact]
        public async void MoveUpCategoryTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            // Create groups
            var res1 = await sr.SetModuleGroupAsync("Group #4", "Description");
            Assert.True(res1.Success);
            var res2 = await sr.SetModuleGroupAsync("Group #5", "Description");
            Assert.True(res2.Success);
            // Check group positions
            var res3 = await sr.GetModuleGroupAsync("Group #4");
            Assert.Equal(1, res3.GroupIndex);
            var res4 = await sr.GetModuleGroupAsync("Group #5");
            Assert.Equal(2, res4.GroupIndex);
            // Move the first group one position down
            await sr.MoveModuleGroupUpAsync("Group #5");
            // Recheck group positions
            var res5 = await sr.GetModuleGroupAsync("Group #4");
            Assert.Equal(2, res5.GroupIndex);
            var res6 = await sr.GetModuleGroupAsync("Group #5");
            Assert.Equal(1, res6.GroupIndex);
        }
    }
}