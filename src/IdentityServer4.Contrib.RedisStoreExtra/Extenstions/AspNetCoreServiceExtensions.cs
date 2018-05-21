using IdentityServer4Extras;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Contrib.RedisStoreExtra.Extenstions
{
    public static class AspNetCoreServiceExtensions
    {
        public static void AddRedisOperationalStoreExtraTypes(this IServiceCollection services)
        {
            services.AddTransient<IdentityServer4.Contrib.RedisStore.Stores.PersistedGrantStore>();
        }
    }
}