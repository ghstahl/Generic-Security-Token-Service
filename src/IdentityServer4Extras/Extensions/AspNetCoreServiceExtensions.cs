using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Extras.Extensions
{
    public static class AspNetCoreServiceExtensions
    {
        public static void AddIdentityServer4ExtraTypes(this IServiceCollection services)
        {
            services.AddTransient<IRawClientSecretValidator, RawClientSecretValidator>();
            services.AddTransient<PrincipalAugmenter>();  
        }
    }
}