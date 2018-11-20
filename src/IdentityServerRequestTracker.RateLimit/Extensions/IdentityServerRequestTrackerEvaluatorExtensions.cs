using IdentityServerRequestTracker;
using IdentityServerRequestTracker.RateLimit.Options;
using IdentityServerRequestTracker.RateLimit.Services;
using IdentityServerRequestTracker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerRequestTracker.RateLimit.Extensions
{
    public static class IdentityServerRequestTrackerEvaluatorExtensions
    {
        public static IServiceCollection AddClientRateLimiter(this IServiceCollection services)
        {
            services.AddTransient<ClientRateLimiterRequestTrackerResult>();
            services.AddSingleton<IIdentityServerRequestTrackerEvaluator, ClientRateLimiterRequestTrackerEvaluator>();
            services.AddSingleton<IClientRateLimitProcessor, RateLimitCore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            


            return services;
        }
        public static void AddClientRateLimiterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ClientRateLimitingOptions>(configuration.GetSection("IdentityServerClientRateLimiting"));
        }
        public static void AddDistributedCacheRateLimitCounterStore(this IServiceCollection services)
        {
            services.AddTransient<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
        }
    }
}