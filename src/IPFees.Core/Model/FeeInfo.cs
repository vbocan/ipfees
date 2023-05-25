using IPFees.Core.Enum;

namespace IPFees.Core.Model
{
    public record FeeInfo(Guid Id, FeeCategory Category, AttorneyFeeLevel AttorneyFeeLevel, string Name, string Description, string SourceCode, IEnumerable<Guid> ReferencedModules, DateTime LastUpdatedOn);
}