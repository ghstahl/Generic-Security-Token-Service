using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryResourceOwnerExtensionGrant.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddArbitraryOwnerResourceExtensionGrant(this IIdentityServerBuilder builder)
        {
            builder
                .AddExtensionGrantValidator<ArbitraryResourceOwnerExtensionGrantValidator>();
            return builder;
        }
    }
}