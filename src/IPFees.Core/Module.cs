using IPFFees.Core.Data;
using IPFFees.Core.Models;
using Mapster;
using MongoDB.Driver;
using System.Diagnostics.Tracing;
using System.Runtime.Versioning;

namespace IPFFees.Core
{
    public class Module : IModule
    {
        private readonly DataContext context;

        public Module(DataContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Create a new module
        /// </summary>
        /// <param name="ModuleName">Module name.</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        /// <remarks>
        /// A module is an IPF source code file that is meant to be included by calling code, akin to the #include functionality of the C programming language.
        /// </remarks>
        public async Task<DbResult> AddModuleAsync(string ModuleName)
        {
            if (GetModules().Any(a => a.Name.Equals(ModuleName)))
            {
                return DbResult.Fail($"A module named '{ModuleName}' already exists.");
            }
            var newDoc = new ModuleDoc
            {
                Name = ModuleName,
                LastUpdatedOn = DateTime.UtcNow.ToLocalTime()
            };
            try
            {
                await context.ModuleCollection.InsertOneAsync(newDoc);
                return DbResult.Succeed(newDoc.Id);
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }
        /// <summary>
        /// Set the module description
        /// </summary>
        /// <param name="ModuleName">Module name</param>
        /// <param name="Description">Description of the functionality provided</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetModuleDescriptionAsync(string ModuleName, string Description)
        {
            var res = await context.ModuleCollection.UpdateOneAsync(r => r.Name.Equals(ModuleName),
                Builders<ModuleDoc>
                .Update
                .Set(r => r.Description, Description));
            return (res.IsAcknowledged && res.ModifiedCount > 0) ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Set the module source code
        /// </summary>
        /// <param name="ModuleName">Module name</param>
        /// <param name="SourceCode">Source code of the module</param>        
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> SetModuleSourceCodeAsync(string ModuleName, string SourceCode)
        {
            var res = await context.ModuleCollection.UpdateOneAsync(r => r.Name.Equals(ModuleName),
                Builders<ModuleDoc>
                .Update
                .Set(r => r.SourceCode, SourceCode));
            return (res.IsAcknowledged && res.ModifiedCount > 0) ? DbResult.Succeed() : DbResult.Fail();
        }

        /// <summary>
        /// Get all registered modules.
        /// </summary>
        /// <returns>An enumeration of ModuleInfo objects</returns>
        public IEnumerable<ModuleInfo> GetModules()
        {
            var dbObjs = context.ModuleCollection.AsQueryable();
            return dbObjs.Adapt<IEnumerable<ModuleInfo>>();
        }

        /// <summary>
        /// Get module by name
        /// </summary>
        /// <param name="ModuleName">Module name</param>
        /// <returns>A ModuleInfo object</returns>
        public ModuleInfo GetModuleByName(string ModuleName)
        {
            var dbObjs = context.ModuleCollection.AsQueryable().Where(w => w.Name.Equals(ModuleName)).Single();
            return dbObjs.Adapt<ModuleInfo>();
        }

        /// <summary>
        /// Remove a specified module
        /// </summary>
        /// <param name="ModuleName">Module name</param>
        /// <returns>A DbResult structure containing the result of the database operation</returns>
        public async Task<DbResult> RemoveModuleAsync(string ModuleName)
        {
            try
            {
                await context.ModuleCollection.DeleteOneAsync(s => s.Name.Equals(ModuleName));
                return DbResult.Succeed();
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }
    }
}