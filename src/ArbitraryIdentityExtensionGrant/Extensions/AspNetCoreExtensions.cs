using IdentityServer4Extras.Services;
using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace ArbitraryIdentityExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryIdentityExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryIdentityRequestValidator>();
        //    services.AddTransient<IProfileServicePlugin, ArbitraryIdentityProfileService>();
            services.AddTransient<ITokenResponseGeneratorHook, TokenResponseGeneratorHook>();
        }
    }
}