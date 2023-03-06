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

        public async Task<DbResult> AddAsync(string name, string description, string code)
        {
            if (GetModules().Any(a => a.Name.Equals(name)))
            {
                return DbResult.Fail($"A module named '{name}' already exists.");
            }
            var newDoc = new ModuleDoc
            {
                Name = name,
                Description = description,
                Code = code
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

        public IEnumerable<ModuleInfo> GetModules()
        {
            var dbObjs = context.ModuleCollection.AsQueryable();
            return dbObjs.Adapt<IEnumerable<ModuleInfo>>();
        }

        public async Task<DbResult> RemoveAsync(string name)
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