using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryNoSubjectExtensionGrant.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddArbitraryNoSubjectExtensionGrant(this IIdentityServerBuilder builder)
        {
            builder
                .AddExtensionGrantValidator<ArbitraryNoSubjectExtensionGrantValidator>();
            return builder;
        }
    }
}