using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public static class GrantExtensions
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
        public static void AddArbitraryOpenIdConnectTokenExtensionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryOpenIdConnectIdentityTokenRequestValidator>();
            services.AddTransient<ProviderValidatorManager>();
            services.AddTransient<IProfileServicePlugin, ArbitraryOpenIdConnectIdentityTokenProfileService>();
        }
    }
}