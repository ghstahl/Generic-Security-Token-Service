using System.Threading.Tasks;

namespace P7IdentityServer4
{
    public interface IKeyVaultCache
    {
        Task<CacheData> GetKeyVaultCacheDataAsync();
    }
}