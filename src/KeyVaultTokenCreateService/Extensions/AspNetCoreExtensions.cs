using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace P7IdentityServer4.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddKeyVaultTokenCreateServiceTypes(this IServiceCollection services)
        {
            services.AddTransient<IPublicKeyProvider, AzureKeyVaultPublicKeyProvider>();
            services.AddSingleton<IKeyVaultCache, KeyVaultCache>();
        }

        public static void AddKeyVaultTokenCreateServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.Configure<AzureKeyVaultTokenSigningServiceOptions>(configuration.GetSection("azureKeyVaultTokenSigningServiceOptions"));
        }
    }
}