using IdentityServerRequestTracker.Services;
using IdentityServerRequestTracker.Usage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerRequestTracker.Usage.Extensions
{
    public static class IdentityServerUsageRequestTrackerEvaluatorExtensions
    {
        public static IServiceCollection AddClientUsageTracker(this IServiceCollection services)
        {
            services.AddSingleton<IIdentityServerRequestTrackerEvaluator, ClientUsageRequestTrackerEvaluator>();
   //         services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            return services;
        }

        public static IServiceCollection AddInMemoryClientUsageStore(this IServiceCollection services)
        {
            services.AddSingleton< IClientUsageStore,InMemoryClientUsageStore>();
            return services;
        }
        public static void AddClientUsageTrackerOptions(this IServiceCollection services, IConfiguration configuration)
        {
   //         services.Configure<ClientRateLimitingOptions>(configuration.GetSection("IdentityServerClientRateLimiting"));
        }
         
    }
}