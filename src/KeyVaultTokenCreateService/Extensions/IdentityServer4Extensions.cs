using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace P7IdentityServer4.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddKeyVaultTokenCreateService(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenCreationService>();
            builder.Services.TryAddTransient<ITokenCreationService, MyDefaultTokenCreationService>();

            builder.Services.RemoveAll<ISigningCredentialStore>();
           
            builder.Services.TryAddSingleton<ISigningCredentialStore>(x => x.GetService<MySigningCredentialStore>());

            builder.Services.RemoveAll<IKeyMaterialService>();
            builder.Services.TryAddSingleton<IKeyMaterialService>(x => x.GetService<MySigningCredentialStore>());

            return builder;
        }
    }
}