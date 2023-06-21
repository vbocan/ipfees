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
            var sr = fixture.SettingsRepository;
            var res1 = await sr.SetAttorneyFeeAsync(AttorneyFeeLevel.Level1, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await sr.GetAttorneyFeeAsync(AttorneyFeeLevel.Level1);
            Assert.Equal(150, res2.Amount);
            Assert.Equal("USD", res2.Currency);
        }

        [Fact]
        public async void SetGetAttorneyFeeConsecutiveTest()
        {
            var sr = fixture.SettingsRepository;
            var res1 = await sr.SetAttorneyFeeAsync(AttorneyFeeLevel.Level2, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await sr.SetAttorneyFeeAsync(AttorneyFeeLevel.Level2, 200, "EUR");
            Assert.True(res2.Success);
            var res3 = await sr.GetAttorneyFeeAsync(AttorneyFeeLevel.Level2);
            Assert.Equal(200, res3.Amount);
            Assert.Equal("EUR", res3.Currency);
        }

        [Fact]
        public async void SetGetAttorneyFeeInvalidTest()
        {
            var sr = fixture.SettingsRepository;
            var res1 = await sr.GetAttorneyFeeAsync(AttorneyFeeLevel.Level3);
            Assert.Equal(0, res1.Amount);
            Assert.Equal(string.Empty, res1.Currency);
        }               
    }
}