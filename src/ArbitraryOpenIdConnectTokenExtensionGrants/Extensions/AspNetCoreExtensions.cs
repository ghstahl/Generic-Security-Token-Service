using IdentityModelExtras;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProfileServiceManager;

namespace ArbitraryOpenIdConnectTokenExtensionGrants.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryOpenIdConnectTokenExtensionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryOpenIdConnectIdentityTokenRequestValidator>();
            services.AddTransient<ProviderValidatorManager>();
            services.AddTransient<IProfileServicePlugin, ArbitraryOpenIdConnectIdentityTokenProfileService>();
            services.TryAddTransient<IDefaultHttpClientFactory, NullDefaultHttpClientFactory>();
        }
    }
}