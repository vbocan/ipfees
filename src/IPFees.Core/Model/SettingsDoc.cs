using IPFees.Core.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IPFees.Core.Model
{
    [BsonIgnoreExtraElements]
    public class SettingsDoc
    {
        public const string CollectionName = "Settings";

        public SettingsDoc() {
            ModuleGroups = new List<ModuleGroup>();
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public List<ModuleGroup> ModuleGroups { get; set; }
        public List<AttorneyFee> AttorneyFeeLevels { get; set; }
    }

    public record ModuleGroup(string GroupName, string GroupDescription);
    public record AttorneyFee(JurisdictionAttorneyFeeLevel Level, double Amount, string Currency);
}
