using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using P7.Core.Scheduler.Scheduling;
using P7IdentityServer4.Cron;

namespace P7IdentityServer4.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddKeyVaultTokenCreateServiceTypes(this IServiceCollection services)
        {
            services.TryAddSingleton<MySigningCredentialStore>();
            services.TryAddSingleton<ITokenSigningCredentialStore>(x => x.GetService<MySigningCredentialStore>());
            services.AddTransient<IPublicKeyProvider, AzureKeyVaultPublicKeyProvider>();
            services.AddSingleton<IKeyVaultCache, KeyVaultCache>();
            services.AddTransient<IScheduledTask, DataRefreshServiceTask>();
            return services;
        }

        public static IServiceCollection AddKeyVaultTokenCreateServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureKeyVaultTokenSigningServiceOptions>(configuration.GetSection("appOptions:keyVault"));
            return services;
        }
    }
}