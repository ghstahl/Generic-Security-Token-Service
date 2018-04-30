using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public static class GrantExtensions
    {
        public static IIdentityServerBuilder AddArbitraryOwnerResourceExtensionGrant(this IIdentityServerBuilder builder)
        {
            builder
                .AddExtensionGrantValidator<ArbitraryResourceOwnerExtensionGrantValidator>()
                .AddProfileService<ArbitraryResourceOwnerProfileService>();
            return builder;
        }

        public static void AddArbitraryResourceOwnerExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryResourceOwnerRequestValidator>();
            services.AddTransient<IProfileServicePlugin, ArbitraryResourceOwnerProfileService>();
        }
    }
}