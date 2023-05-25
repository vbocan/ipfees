using IPFees.Core.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFees.Core.Data
{
    public class DataContext
    {
        public IMongoDatabase Context { get; private set; }
        public IMongoCollection<ModuleDoc> ModuleCollection { get; set; }
        public IMongoCollection<JurisdictionDoc> JurisdictionCollection { get; set; }
        public IMongoCollection<FeeDoc> FeeCollection { get; set; }        
        public IMongoCollection<AttorneyFeesDoc> AttorneyFeesCollection { get; set; }

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
            FeeCollection = Context.GetCollection<FeeDoc>(FeeDoc.CollectionName);
            JurisdictionCollection = Context.GetCollection<JurisdictionDoc>(JurisdictionDoc.CollectionName);
            AttorneyFeesCollection = Context.GetCollection<AttorneyFeesDoc>(AttorneyFeesDoc.CollectionName);
            // Ensure indexes are in place
            /*
            var actionLogBuilder = Builders<ActionLogDoc>.IndexKeys;
            var indexModel = new CreateIndexModel<ActionLogDoc>(actionLogBuilder.Ascending(x => x.EventCode).Ascending(x => x.Timestamp).Ascending(x => x.User));
            ActionLogCollection.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
            */
        }

        public void DropDatabase()
        {
            var client = new MongoClient(ConnectionString);
            client.DropDatabase(DatabaseName);
        }
    }
}
