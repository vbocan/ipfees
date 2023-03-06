using IPFees.Core.Helpers;
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
        private readonly IDateTimeHelper dateTimeHelper;

        public Module(DataContext context, IDateTimeHelper dateTimeHelper)
        {
            this.context = context;
            this.dateTimeHelper = dateTimeHelper;
        }

        public async Task<DbResult> AddModuleAsync(string name, string description, string code)
        {
            if (GetModules().Any(a => a.Name.Equals(name)))
            {
                return DbResult.Fail($"A module named '{name}' already exists.");
            }
            var newDoc = new ModuleDoc
            {
                Name = name,
                Description = description,
                Code = code,
                LastUpdatedOn = dateTimeHelper.GetDateTimeNow()
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

        public async Task<DbResult> EditModuleAsync(string name, string description, string code)
        {
            var res = await context.ModuleCollection.UpdateOneAsync(r => r.Name.Equals(name),
                Builders<ModuleDoc>
                .Update
                .Set(r => r.Description, description)
                .Set(r => r.Code, code));
            return (res.IsAcknowledged && res.ModifiedCount > 0) ? DbResult.Succeed() : DbResult.Fail();
        }

        public IEnumerable<ModuleInfo> GetModules()
        {
            var dbObjs = context.ModuleCollection.AsQueryable();
            return dbObjs.Adapt<IEnumerable<ModuleInfo>>();
        }

        public async Task<DbResult> RemoveModuleAsync(string name)
        {
            try
            {
                await context.ModuleCollection.DeleteOneAsync(s => s.Name.Equals(name));
                return DbResult.Succeed();
            }
            catch (Exception ex)
            {
                return DbResult.Fail(ex);
            }
        }
    }
}