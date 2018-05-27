using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using P7.Core.Scheduler.Scheduling;
using P7IdentityServer4.Cron;

namespace P7IdentityServer4.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddKeyVaultTokenCreateServiceTypes(this IServiceCollection services)
        {
            services.AddTransient<IPublicKeyProvider, AzureKeyVaultPublicKeyProvider>();
            services.AddSingleton<IKeyVaultCache, KeyVaultCache>();
            services.AddTransient<IScheduledTask, DataRefreshServiceTask>();
            
        }

        public static void AddKeyVaultTokenCreateServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureKeyVaultTokenSigningServiceOptions>(configuration.GetSection("azureKeyVaultTokenSigningServiceOptions"));
        }
    }
}