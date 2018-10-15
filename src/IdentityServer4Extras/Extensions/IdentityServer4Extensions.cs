using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Services;
using IdentityServer4Extras.Stores;
using IdentityServer4Extras.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4Extras.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddNoSecretRefreshClientSecretValidator(this IIdentityServerBuilder builder)
        {
            builder.Services.Remove<IClientSecretValidator>();
            builder.Services.TryAddTransient<IClientSecretValidator, NoSecretRefreshClientSecretValidator>();

            builder.Services.Remove<IClientSecretValidator>();
            builder.Services.TryAddTransient<IClientSecretValidator, NoSecretRefreshClientSecretValidator>();
          
            return builder;
        }
        public static IIdentityServerBuilder AddInMemoryPersistedGrantStoreExtra(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IPersistedGrantStore>();
            builder.Services.TryAddSingleton<InMemoryPersistedGrantStore>();
            builder.Services.TryAddSingleton<IPersistedGrantStore, PersistedGrantStoreExtra>();
            return builder;
        }
        public static IIdentityServerBuilder AddInMemoryClientStoreExtra(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IClientStore>();
            builder.Services.TryAddSingleton<IClientStoreExtra, InMemoryClientStoreExtra>();
            builder.Services.TryAddSingleton<IClientStore>(x => x.GetService<IClientStoreExtra>());
            return builder;
        }

        public static IIdentityServerBuilder AddNullRefreshTokenKeyObfuscator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IRefreshTokenKeyObfuscator>();
            builder.Services.AddTransient<IRefreshTokenKeyObfuscator, NullRefreshTokenKeyObfuscator>();
            return builder;
        }
        public static IIdentityServerBuilder AddProtectedRefreshTokenKeyObfuscator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IRefreshTokenKeyObfuscator>();
            builder.Services.AddTransient<IRefreshTokenKeyObfuscator, ProtectedRefreshTokenKeyObfuscator>();
            return builder;
        }
    }
}
