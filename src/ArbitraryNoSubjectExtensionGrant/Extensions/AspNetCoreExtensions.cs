using Microsoft.Extensions.DependencyInjection;

namespace ArbitraryNoSubjectExtensionGrant.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddArbitraryNoSubjectExtentionGrantTypes(this IServiceCollection services)
        {
            services.AddTransient<ArbitraryNoSubjectRequestValidator>();
        }
    }
}