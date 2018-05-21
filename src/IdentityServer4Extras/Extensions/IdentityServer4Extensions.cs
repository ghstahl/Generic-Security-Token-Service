using IdentityServer4.Stores;
using IdentityServer4.Validation;
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
        public static IIdentityServerBuilder AddInMemoryPersistedGrantStore2(this IIdentityServerBuilder builder)
        {
            builder.Services.Remove<IPersistedGrantStore>();
            builder.Services.TryAddSingleton<IPersistedGrantStore2, InMemoryPersistedGrantStore2>();
            builder.Services.TryAddSingleton<IPersistedGrantStore>(x => x.GetService<IPersistedGrantStore2>());

            return builder;
        }
    }
}
