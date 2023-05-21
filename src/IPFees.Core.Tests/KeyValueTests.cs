using IPFees.Core.Tests.Fixture;
using IPFees.Core;
using IPFees.Core.Data;
using IPFees.Core.Repository;
using IPFees.Core.Enum;

namespace IPFees.Core.Tests
{
    public class KeyValueTests : IClassFixture<CoreFixture>
    {
        private readonly CoreFixture fixture;

        public KeyValueTests(CoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void SetGetAttorneyFeeTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level1, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await kv.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level1);
            Assert.Equal(150, res2.Item1);
            Assert.Equal("USD", res2.Item2);
        }

        [Fact]
        public async void SetGetAttorneyFeeConsecutiveTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2, 150, "USD");
            Assert.True(res1.Success);
            var res2 = await kv.SetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2, 200, "EUR");
            Assert.True(res2.Success);
            var res3 = await kv.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level2);
            Assert.Equal(200, res3.Item1);
            Assert.Equal("EUR", res3.Item2);
        }

        [Fact]
        public async void SetGetAttorneyFeeInvalidTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.GetAttorneyFeeAsync(JurisdictionAttorneyFeeLevel.Level3);
            Assert.Equal(0, res1.Item1);
            Assert.Equal(string.Empty, res1.Item2);
        }

        [Fact]
        public async void SetGetCategoryTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.SetCategoryAsync("Category #1", 100, "Description");
            Assert.True(res1.Success);
            var res2 = await kv.GetCategoryAsync("Category #1");
            Assert.Equal(100, res2.Item1);
            Assert.Equal("Description", res2.Item2);
        }

        [Fact]
        public async void SetGetCategoryConsecutiveTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.SetCategoryAsync("Category #2", 100, "Description");
            Assert.True(res1.Success);
            var res2 = await kv.SetCategoryAsync("Category #2", 150, "Description2");
            Assert.True(res2.Success);
            var res3 = await kv.GetCategoryAsync("Category #2");
            Assert.Equal(150, res3.Item1);
            Assert.Equal("Description2", res3.Item2);
        }

        [Fact]
        public async void SetGetCategoryInvalidTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res = await kv.GetCategoryAsync("Category #3");
            Assert.Equal(0, res.Item1);
            Assert.Equal(string.Empty, res.Item2);
        }
    }
}