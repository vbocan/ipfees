using IPFFees.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFFees.Core.Data
{
    public class DataContext
    {
        public IMongoDatabase Context { get; private set; }
        public IMongoCollection<ModuleDoc> ModuleCollection { get; set; }
        public IMongoCollection<JurisdictionDoc> JurisdictionCollection { get; set; }

        private string ConnectionString { get; set; }
        private string DatabaseName { get; set; }

        public DataContext(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            DatabaseName = MongoUrl.Create(ConnectionString).DatabaseName;
            var client = new MongoClient(ConnectionString);
            if (client != null)
                Context = client.GetDatabase(DatabaseName);
            
            ModuleCollection = Context.GetCollection<ModuleDoc>(ModuleDoc.CollectionName);
            JurisdictionCollection = Context.GetCollection<JurisdictionDoc>(JurisdictionDoc.CollectionName);
        }

        public void DropDatabase()
        {
            var client = new MongoClient(ConnectionString);
            client.DropDatabase(DatabaseName);
        }
    }
}
