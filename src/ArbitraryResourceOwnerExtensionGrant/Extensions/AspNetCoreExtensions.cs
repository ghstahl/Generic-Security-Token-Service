using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace ArbitraryResourceOwnerExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryResourceOwnerExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryResourceOwnerRequestValidator>();
            services.AddTransient<IProfileServicePlugin, ArbitraryResourceOwnerProfileService>();
            services.AddSingleton<OIDCDiscoverCacheContainer>();
        }
    }
}