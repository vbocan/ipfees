using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class ModuleDoc
    {
        public const string CollectionName = "Modules";

        public ModuleDoc()
        {
            Name = string.Empty;
            Description = string.Empty;
            SourceCode = string.Empty;
            GroupName = string.Empty;            
            LastUpdatedOn = DateTime.MinValue;
        }

        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }        
        public string SourceCode { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdatedOn { get; set; }
    }
}
