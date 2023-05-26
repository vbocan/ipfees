using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record FeeInfo(Guid Id, FeeCategory Category, string Name, string Description, string JurisdictionName, string SourceCode, IEnumerable<Guid> ReferencedModules, DateTime LastUpdatedOn);
}