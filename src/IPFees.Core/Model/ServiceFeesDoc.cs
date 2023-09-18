using IPFees.Core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class ServiceFeesDoc
    {
        public const string CollectionName = "ServiceFees";

        public ServiceFeesDoc() {
            FeeLevel = ServiceFeeLevel.Level1;
            Amount = 0;
            Currency = string.Empty;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ServiceFeeLevel FeeLevel { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
