using IPFees.Core.Data;
using IPFees.Core.Model;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IPFees.Core.Repository
{
    public class KeyValueRepository : IKeyValueRepository
    {
        private readonly DataContext context;

        public KeyValueRepository(DataContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Set key to specified value
        /// </summary>        
        /// <param name="Key">Key</param>
        /// <param name="Value">Value</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetKeyAsync(string Key, int Value)
        {
            var res = await context.KeyValueCollection.UpdateOneAsync(r => r.Key.Equals(Key),
                Builders<KeyValueDoc>
                .Update
                .Set(r => r.Value, Value), new UpdateOptions { IsUpsert = true });
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Get the value of a key
        /// </summary>
        /// <param name="Key">Key</param>
        /// <returns>Value or 0 if the key has not been previously set</returns>
        public async Task<int> GetKeyAsync(string Key)
        {
            var res = (await context.KeyValueCollection.FindAsync(r => r.Key.Equals(Key))).SingleOrDefault();
            if (res != null)
            {
                return res.Value;
            }
            else { return 0; }
        }

    }
}