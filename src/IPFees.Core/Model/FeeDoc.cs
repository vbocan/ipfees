using IPFees.Core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class FeeDoc
    {
        public const string CollectionName = "Fees";

        public FeeDoc() {
            Category = FeeCategory.OfficialFees;
            Name = string.Empty;
            Description = string.Empty;
            SourceCode = string.Empty;
            JurisdictionName = string.Empty;
            ReferencedModules = new List<Guid>();
            LastUpdatedOn = DateTime.MinValue;
        }

        [BsonId]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public FeeCategory Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string JurisdictionName { get; set; }
        public string SourceCode { get; set; }
        public IList<Guid> ReferencedModules { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdatedOn { get; set; }
    }    
}
