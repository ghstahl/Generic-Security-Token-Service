using IdentityServer4.Validation;
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
            return builder;
        }
    }
}
