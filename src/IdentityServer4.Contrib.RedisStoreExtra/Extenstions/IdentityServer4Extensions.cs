using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Stores;
using IdentityServer4Extras.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Contrib.RedisStoreExtra.Extenstions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddRedisOperationalStoreExtra(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IPersistedGrantStore>();
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStoreExtra>();
            return builder;
        }
    }
}
