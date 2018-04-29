using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Extras
{
    public static class AspNetCoreServiceExtensions
    {
        public static void AddIdentityServer4ExtraTypes(this IServiceCollection services)
        {
            services.AddTransient<IRawClientSecretValidator, RawClientSecretValidator>();
            services.AddTransient<IClientStoreExtra, InMemoryClientStoreExtra>();
            services.AddTransient<PrincipalAugmenter>();  
        }
    }
}