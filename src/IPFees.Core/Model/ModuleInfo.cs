namespace IPFees.Core.Model
{
    public record ModuleInfo(Guid Id, string Name, string Description, string GroupName, string SourceCode, DateTime LastUpdatedOn);
}