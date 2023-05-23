using IPFees.Core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class AttorneyFeesDoc
    {
        public const string CollectionName = "AttorneyFees";

        public AttorneyFeesDoc() {
            FeeLevel = JurisdictionAttorneyFeeLevel.Level1;
            Amount = 0;
            Currency = string.Empty;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public JurisdictionAttorneyFeeLevel FeeLevel { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
