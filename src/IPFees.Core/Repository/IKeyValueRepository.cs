using IPFees.Core.Data;

namespace IPFees.Core.Repository
{
    public interface IKeyValueRepository
    {
        Task<DbResult> SetKeyAsync(string Key, int Value);
        Task<int> GetKeyAsync(string Key);
    }
}