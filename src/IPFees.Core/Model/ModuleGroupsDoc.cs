using IPFees.Core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class ModuleGroupsDoc
    {
        public const string CollectionName = "ModuleGroups";

        public ModuleGroupsDoc()
        {
            GroupName = string.Empty;
            GroupDescription = string.Empty;
            GroupWeight = 1;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int GroupWeight { get; set; }
    }
}
