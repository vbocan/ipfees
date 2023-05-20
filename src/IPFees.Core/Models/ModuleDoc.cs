using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Models
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
            Category = string.Empty;
            Weight = 100;
            LastUpdatedOn = DateTime.MinValue;
        }

        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Weight { get; set; }
        public string SourceCode { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdatedOn { get; set; }
    }
}
