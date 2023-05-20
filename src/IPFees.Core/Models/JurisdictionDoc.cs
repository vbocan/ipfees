using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Models
{
    [BsonIgnoreExtraElements]
    public class JurisdictionDoc
    {
        public const string CollectionName = "Jurisdictions";

        public JurisdictionDoc() {
            Category = JurisdictionCategory.OfficialFees;
            Name = string.Empty;
            Description = string.Empty;
            SourceCode = string.Empty;
            ReferencedModules = new List<Guid>();
            LastUpdatedOn = DateTime.MinValue;
        }

        [BsonId]
        public Guid Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public JurisdictionCategory Category { get; set; }
        [BsonRepresentation(BsonType.String)]
        public JurisdictionAttorneyFeeLevel AttorneyFeeLevel { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceCode { get; set; }
        public IList<Guid> ReferencedModules { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdatedOn { get; set; }
    }    
}
