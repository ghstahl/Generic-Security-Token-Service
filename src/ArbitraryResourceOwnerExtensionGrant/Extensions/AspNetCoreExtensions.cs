using IdentityServer4Extras.Services;
using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace ArbitraryResourceOwnerExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryResourceOwnerExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryResourceOwnerRequestValidator>();
            services.AddSingleton<OIDCDiscoverCacheContainer>();
            services.AddTransient<ITokenServiceHookPlugin, TokenServiceHookPlugin>();
        }
    }
}