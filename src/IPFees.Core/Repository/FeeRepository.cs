using IPFees.Calculator;
using IPFees.Evaluator;
using IPFees.Core.Data;
using IPFees.Core.Model;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using IPFees.Core.Enum;

namespace IPFees.Core.Repository
{
    public class FeeRepository : IFeeRepository
    {
        private readonly DataContext context;

        public FeeRepository(DataContext context)
        {
            this.context = context;
        }

        public FeeRepository()
        {
            context = null!;
        }

        /// <summary>
        /// Create a new fee
        /// </summary>
        /// <param name="Name">Fee name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        /// <remarks>
        /// A fee is an area where Intellectual Property rules are in effect and usually consists of one or more countries.        
        /// </remarks>
        public async Task<DbResult> AddFeeAsync(string Name)
        {
            var Fees = await GetFees();
            if (Fees.Any(a => a.Name.Equals(Name)))
            {
                return DbResult.Fail($"A fee named '{Name}' already exists.");
            }
            var newDoc = new FeeDoc
            {
                Name = Name,
                LastUpdatedOn = DateTime.UtcNow.ToLocalTime()
            };
            try
            {
                await context.FeeCollection.InsertOneAsync(newDoc);
                return DbResult.Succeed(newDoc.Id);
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }

        /// <summary>
        /// Set the fee category
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <param name="Category">Fee category</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetFeeCategoryAsync(Guid Id, FeeCategory Category)
        {
            var res = await context.FeeCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<FeeDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.Category, Category));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }
        
        /// <summary>
        /// Set the fee name
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <param name="Name">Fee name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetFeeNameAsync(Guid Id, string Name)
        {
            var res = await context.FeeCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<FeeDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.Name, Name));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the name of the jurisdiction associated to the fee
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <param name="JurisdictionName">Jurisdiction name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetFeeJurisdictionNameAsync(Guid Id, string JurisdictionName)
        {
            var res = await context.FeeCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<FeeDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.JurisdictionName, JurisdictionName));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the fee description
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <param name="Description">Description of the functionality provided</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetFeeDescriptionAsync(Guid Id, string Description)
        {
            var res = await context.FeeCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<FeeDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.Description, Description));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the fee source code
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <param name="SourceCode">Source code of the fee</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetFeeSourceCodeAsync(Guid Id, string SourceCode)
        {
            var res = await context.FeeCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<FeeDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.SourceCode, SourceCode));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Get all registered fees.
        /// </summary>
        /// <returns>An enumeration of FeeInfo objects</returns>
        public async Task<IEnumerable<FeeInfo>> GetFees()
        {
            var dbObjs = await context.FeeCollection.FindAsync(new BsonDocument());
            return dbObjs.ToList().Adapt<IEnumerable<FeeInfo>>();
        }

        /// <summary>
        /// Get fee by Id
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <returns>A FeeInfo object</returns>
        public async Task<FeeInfo> GetFeeById(Guid Id)
        {
            var filter = Builders<FeeDoc>.Filter.Eq(m => m.Id, Id);
            var dbObjs = (await context.FeeCollection.FindAsync(filter)).FirstOrDefaultAsync().Result;
            return dbObjs.Adapt<FeeInfo>();
        }

        /// <summary>
        /// Remove a specified fee
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> RemoveFeeAsync(Guid Id)
        {
            try
            {
                await context.FeeCollection.DeleteOneAsync(s => s.Id.Equals(Id));
                return DbResult.Succeed();
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }

        /// <summary>
        /// Set the modules referenced by the fee
        /// </summary>
        /// <param name="Id">Fee id</param>
        /// <param name="ModuleIds">An array of module ids referenced by the fee</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetReferencedModules(Guid Id, IList<Guid> ModuleIds)
        {
            var res = await context.FeeCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<FeeDoc>
                .Update
                .Set(r => r.ReferencedModules, ModuleIds));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }
    }
}