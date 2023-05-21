using IPFees.Core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class KeyValueDoc
    {
        public const string CollectionName = "KeyValue";

        public KeyValueDoc() {
            Key = string.Empty;
            Value = 0;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }
    }    
}
