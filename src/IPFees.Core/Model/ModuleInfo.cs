namespace IPFees.Core.Model
{
    public record ModuleInfo(Guid Id, string Name, string Description, string Category, string SourceCode, DateTime LastUpdatedOn);
}