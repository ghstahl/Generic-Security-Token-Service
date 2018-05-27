using System.Threading;
using System.Threading.Tasks;

namespace P7IdentityServer4
{
    public interface IKeyVaultCache
    {
        Task<CacheData> GetKeyVaultCacheDataAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task RefreshCacheFromSourceAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}