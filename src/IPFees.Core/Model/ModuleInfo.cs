namespace IPFees.Core.Model
{
    public record ModuleInfo(Guid Id, string Name, string Description, string SourceCode, bool AutoRun, DateTime LastUpdatedOn);
}