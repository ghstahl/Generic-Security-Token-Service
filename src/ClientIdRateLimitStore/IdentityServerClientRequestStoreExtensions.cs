using AspNetCoreRateLimit.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClientIdRateLimitStore
{
    public static class IdentityServerClientRequestStoreExtensions
    {
        public static void AddIdentityServerClientRequestStore(this IServiceCollection services)
        {
            services.RemoveAll<IClientRequestStore>();
            services.TryAddSingleton<IClientRequestStore, IdentityServerClientRequestStore>();
        }
    }
}