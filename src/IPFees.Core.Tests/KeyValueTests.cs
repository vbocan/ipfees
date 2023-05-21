using IPFees.Core.Tests.Fixture;
using IPFees.Core;
using IPFees.Core.Data;
using IPFees.Core.Repository;

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
        public async void SetGetKeyTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.SetKeyAsync("Key1", 100);
            Assert.True(res1.Success);
            var res2 = await kv.GetKeyAsync("Key1");
            Assert.Equal(100, res2);
        }

        [Fact]
        public async void ConsecutiveSetKeyTest()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.SetKeyAsync("Key1", 100);
            Assert.True(res1.Success);
            var res2 = await kv.SetKeyAsync("Key1", 120);
            Assert.True(res2.Success);
            var res3 = await kv.GetKeyAsync("Key1");
            Assert.Equal(120, res3);
        }

        [Fact]
        public async void GetInvalidKey()
        {
            var kv = new KeyValueRepository(fixture.DbContext);
            var res1 = await kv.GetKeyAsync("UnknownKey");
            Assert.Equal(0, res1);
        }
    }
}