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
            var kv = new SettingsRepository(fixture.DbContext);
            var res1 = await kv.SetAttorneyFeeLevelAsync(JurisdictionAttorneyFeeLevel.Level1, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await kv.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level1);
            Assert.Equal(150, res2.Item1);
            Assert.Equal("USD", res2.Item2);
        }

        [Fact]
        public async void SetGetAttorneyFeeConsecutiveTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            var res1 = await kv.SetAttorneyFeeLevelAsync(JurisdictionAttorneyFeeLevel.Level2, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await kv.SetAttorneyFeeLevelAsync(JurisdictionAttorneyFeeLevel.Level2, 200, "EUR");
            Assert.True(res2.Success);
            var res3 = await kv.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2);
            Assert.Equal(200, res3.Item1);
            Assert.Equal("EUR", res3.Item2);
        }

        [Fact]
        public async void SetGetAttorneyFeeInvalidTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            var res1 = await kv.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level3);
            Assert.Equal(0, res1.Item1);
            Assert.Equal(string.Empty, res1.Item2);
        }

        [Fact]
        public async void SetGetCategoryTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            var res1 = await kv.SetModuleGroupAsync("Group #1", "Description");
            Assert.True(res1.Success);
            var res2 = await kv.GetModuleGroupAsync("Group #1");            
            Assert.Equal("Description", res2.Item1);
            Assert.Equal(1, res2.Item2);
        }

        [Fact]
        public async void SetGetCategoryConsecutiveTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            var res1 = await kv.SetModuleGroupAsync("Group #2", "Description");
            Assert.True(res1.Success);
            var res2 = await kv.SetModuleGroupAsync("Group #2", "Altered Description");
            Assert.True(res2.Success);
            var res3 = await kv.GetModuleGroupAsync("Group #2");
            Assert.Equal("Altered Description", res3.Item1);
            Assert.Equal(1, res3.Item2);
        }

        [Fact]
        public async void SetGetCategoryInvalidTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            var res1 = await kv.GetModuleGroupAsync("Group #3");
            Assert.Equal(string.Empty, res1.Item1);
            Assert.Equal(-1, res1.Item2);
        }

        [Fact]
        public async void MoveDownCategoryTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            // Create groups
            var res1 = await kv.SetModuleGroupAsync("Group #4", "Description");
            Assert.True(res1.Success);
            var res2 = await kv.SetModuleGroupAsync("Group #5", "Description");
            Assert.True(res2.Success);
            // Check group positions
            var res3 = await kv.GetModuleGroupAsync("Group #4");            
            Assert.Equal(1, res3.Item2);
            var res4 = await kv.GetModuleGroupAsync("Group #5");
            Assert.Equal(2, res4.Item2);
            // Move the first group one position down
            await kv.MoveModuleGroupDownAsync("Group #4");
            // Recheck group positions
            var res5 = await kv.GetModuleGroupAsync("Group #4");
            Assert.Equal(2, res5.Item2);
            var res6 = await kv.GetModuleGroupAsync("Group #5");
            Assert.Equal(1, res6.Item2);
        }

        [Fact]
        public async void MoveUpCategoryTest()
        {
            var kv = new SettingsRepository(fixture.DbContext);
            // Create groups
            var res1 = await kv.SetModuleGroupAsync("Group #4", "Description");
            Assert.True(res1.Success);
            var res2 = await kv.SetModuleGroupAsync("Group #5", "Description");
            Assert.True(res2.Success);
            // Check group positions
            var res3 = await kv.GetModuleGroupAsync("Group #4");
            Assert.Equal(1, res3.Item2);
            var res4 = await kv.GetModuleGroupAsync("Group #5");
            Assert.Equal(2, res4.Item2);
            // Move the first group one position down
            await kv.MoveModuleGroupUpAsync("Group #5");
            // Recheck group positions
            var res5 = await kv.GetModuleGroupAsync("Group #4");
            Assert.Equal(2, res5.Item2);
            var res6 = await kv.GetModuleGroupAsync("Group #5");
            Assert.Equal(1, res6.Item2);
        }
    }
}