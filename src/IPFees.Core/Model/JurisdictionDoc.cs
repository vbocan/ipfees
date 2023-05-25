using IPFees.Core.Enum;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class JurisdictionDoc
    {
        public const string CollectionName = "Jurisdictions";

        public JurisdictionDoc()
        {
            Name = string.Empty;
            Description = string.Empty;
            AttorneyFeeLevel = AttorneyFeeLevel.Level1;
            LastUpdatedOn = DateTime.MinValue;
        }

        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
        public AttorneyFeeLevel AttorneyFeeLevel { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdatedOn { get; set; }
    }
}
