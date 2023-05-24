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
            var res1 = await sr.SetModuleGroupAsync("Group #1", "Description", 100);
            Assert.True(res1.Success);
            var res2 = await sr.GetModuleGroupAsync("Group #1");
            Assert.Equal("Description", res2.GroupDescription);
            Assert.Equal(100, res2.GroupWeight);
        }

        [Fact]
        public async void SetGetCategoryConsecutiveTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.SetModuleGroupAsync("Group #2", "Description", 200);
            Assert.True(res1.Success);
            var res2 = await sr.SetModuleGroupAsync("Group #2", "Altered Description", 300);
            Assert.True(res2.Success);
            var res3 = await sr.GetModuleGroupAsync("Group #2");
            Assert.Equal("Altered Description", res3.GroupDescription);
            Assert.Equal(300, res3.GroupWeight);
        }

        [Fact]
        public async void SetGetCategoryInvalidTest()
        {
            var sr = new SettingsRepository(fixture.DbContext);
            var res1 = await sr.GetModuleGroupAsync("Group #3");
            Assert.Equal(string.Empty, res1.GroupDescription);
            Assert.Equal(-1, res1.GroupWeight);
        }       
    }
}