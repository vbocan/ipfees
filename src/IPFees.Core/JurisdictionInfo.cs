using IPFees.Core.Models;

namespace IPFees.Core
{
    public record JurisdictionInfo(Guid Id, JurisdictionCategory Category, string Name, string Description, string SourceCode, IEnumerable<Guid> ReferencedModules, DateTime LastUpdatedOn);
}