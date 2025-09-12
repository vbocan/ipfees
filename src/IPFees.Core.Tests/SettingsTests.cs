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
        public async Task SetGetServiceFeeTest()
        {
            var sr = fixture.SettingsRepository;
            var res1 = await sr.SetServiceFeeAsync(ServiceFeeLevel.Level1, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await sr.GetServiceFeeAsync(ServiceFeeLevel.Level1);
            Assert.Equal(150, res2.Amount);
            Assert.Equal("USD", res2.Currency);
        }

        [Fact]
        public async Task SetGetServiceFeeConsecutiveTest()
        {
            var sr = fixture.SettingsRepository;
            var res1 = await sr.SetServiceFeeAsync(ServiceFeeLevel.Level2, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await sr.SetServiceFeeAsync(ServiceFeeLevel.Level2, 200, "EUR");
            Assert.True(res2.Success);
            var res3 = await sr.GetServiceFeeAsync(ServiceFeeLevel.Level2);
            Assert.Equal(200, res3.Amount);
            Assert.Equal("EUR", res3.Currency);
        }

        [Fact]
        public async Task SetGetServiceFeeInvalidTest()
        {
            var sr = fixture.SettingsRepository;
            var res1 = await sr.GetServiceFeeAsync(ServiceFeeLevel.Level3);
            Assert.Equal(0, res1.Amount);
            Assert.Equal(string.Empty, res1.Currency);
        }               
    }
}