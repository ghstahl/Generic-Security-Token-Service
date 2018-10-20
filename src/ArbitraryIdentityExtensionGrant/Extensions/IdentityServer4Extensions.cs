using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryIdentityExtensionGrant.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddArbitraryIdentityExtensionGrant(this IIdentityServerBuilder builder)
        {
            builder
                .AddExtensionGrantValidator<ArbitraryIdentityExtensionGrantValidator>();
            return builder;
        }
    }
}