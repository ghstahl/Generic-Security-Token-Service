using IdentityServer4Extras.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryNoSubjectExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryNoSubjectExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryNoSubjectRequestValidator>();
            services.AddTransient<ITokenServiceHookPlugin, TokenServiceHookPlugin>();
        }
    }
}