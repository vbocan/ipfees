using IPFees.Calculator;
using IPFees.Core;
using IPFees.Evaluator;
using IPFees.Core.Data;
using IPFees.Core.Models;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.Tracing;
using System.Runtime.Versioning;
using ZstdSharp;
using static System.Net.Mime.MediaTypeNames;

namespace IPFees.Core
{
    public class JurisdictionRepository : IJurisdictionRepository
    {
        private readonly DataContext context;

        public JurisdictionRepository(DataContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Create a new jurisdiction
        /// </summary>
        /// <param name="Name">Jurisdiction name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        /// <remarks>
        /// A jurisdiction is an area where Intellectual Property rules are in effect and usually consists of one or more countries.        
        /// </remarks>
        public async Task<DbResult> AddJurisdictionAsync(string Name)
        {
            var Jurisdictions = await GetJurisdictions();
            if (Jurisdictions.Any(a => a.Name.Equals(Name)))
            {
                return DbResult.Fail($"A jurisdiction named '{Name}' already exists.");
            }
            var newDoc = new JurisdictionDoc
            {
                Name = Name,
                LastUpdatedOn = DateTime.UtcNow.ToLocalTime()
            };
            try
            {
                await context.JurisdictionCollection.InsertOneAsync(newDoc);
                return DbResult.Succeed(newDoc.Id);
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }

        /// <summary>
        /// Set the jurisdiction category
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <param name="Category">Jurisdiction category</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetJurisdictionCategoryAsync(Guid Id, JurisdictionCategory Category)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.Category, Category));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the jurisdiction name
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <param name="Name">Jurisdiction name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetJurisdictionNameAsync(Guid Id, string Name)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.Name, Name));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the jurisdiction description
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <param name="Description">Description of the functionality provided</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetJurisdictionDescriptionAsync(Guid Id, string Description)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.Description, Description));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the jurisdiction source code
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <param name="SourceCode">Source code of the jurisdiction</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetJurisdictionSourceCodeAsync(Guid Id, string SourceCode)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.LastUpdatedOn, DateTime.UtcNow.ToLocalTime())
                .Set(r => r.SourceCode, SourceCode));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Get all registered jurisdictions.
        /// </summary>
        /// <returns>An enumeration of JurisdictionInfo objects</returns>
        public async Task<IEnumerable<JurisdictionInfo>> GetJurisdictions()
        {
            var dbObjs = await context.JurisdictionCollection.FindAsync(new BsonDocument());
            return dbObjs.ToList().Adapt<IEnumerable<JurisdictionInfo>>();
        }

        /// <summary>
        /// Get jurisdiction by Id
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <returns>A JurisdictionInfo object</returns>
        public async Task<JurisdictionInfo> GetJurisdictionById(Guid Id)
        {
            var filter = Builders<JurisdictionDoc>.Filter.Eq(m => m.Id, Id);
            var dbObjs = (await context.JurisdictionCollection.FindAsync(filter)).FirstOrDefaultAsync().Result;
            return dbObjs.Adapt<JurisdictionInfo>();
        }

        /// <summary>
        /// Remove a specified jurisdiction
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> RemoveJurisdictionAsync(Guid Id)
        {
            try
            {
                await context.JurisdictionCollection.DeleteOneAsync(s => s.Id.Equals(Id));
                return DbResult.Succeed();
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }

        /// <summary>
        /// Set the modules referenced by the jurisdiction
        /// </summary>
        /// <param name="Id">Jurisdiction id</param>
        /// <param name="ModuleIds">An array of module ids referenced by the jurisdiction</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetReferencedModules(Guid Id, IList<Guid> ModuleIds)
        {
            var res = await context.JurisdictionCollection.UpdateOneAsync(r => r.Id.Equals(Id),
                Builders<JurisdictionDoc>
                .Update
                .Set(r => r.ReferencedModules, ModuleIds));
            return res.IsAcknowledged ? DbResult.Succeed() : DbResult.Fail();
        }
    }
}