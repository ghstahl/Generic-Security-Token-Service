using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace ArbitraryOpenIdConnectTokenExtensionGrants.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddArbitraryOpenIdConnectTokenExtensionGrant(this IIdentityServerBuilder builder)
        {
            builder
                .AddExtensionGrantValidator<ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator>();
            return builder;
        }
        public static IIdentityServerBuilder AddArbitraryOpenIdConnectTokenExtensionGrantPassThroughProfileService(this IIdentityServerBuilder builder)
        {
            builder
                .AddProfileService<ArbitraryOpenIdConnectIdentityTokenProfileService>();
            return builder;
        }
    }
}