using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public static class GrantExtensions
    {
        public static IIdentityServerBuilder AddArbitraryOpenIdConnectTokenExtensionGrant(this IIdentityServerBuilder builder)
        {
            builder
                .AddExtensionGrantValidator<ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator>()
                .AddProfileService<ArbitraryOpenIdConnectIdentityTokenProfileService>();
            return builder;
        }

        public static void AddArbitraryOpenIdConnectTokenExtensionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryOpenIdConnectIdentityTokenRequestValidator>();
        }
    }
}