using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFFees.Core.Models
{
    [BsonIgnoreExtraElements]
    public class JurisdictionDoc
    {
        public const string CollectionName = "Jurisdictions";

        public JurisdictionDoc() {
            Name = string.Empty;
            Description = string.Empty;
            SourceCode = string.Empty;
            LastUpdatedOn = DateTime.MinValue;
        }

        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceCode { get; set; }
        public Guid[] ReferencedModules { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdatedOn { get; set; }
    }
}
