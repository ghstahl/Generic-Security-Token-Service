using IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Options;
using IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Extensions
{
    public static class IdentityServerRequestTrackerEvaluatorExtensions
    {
        public static void AddClientRateLimiterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ClientRateLimitingOptions>(configuration.GetSection("IdentityServerClientRateLimiting2"));
        }
        public static void AddDistributedCacheRateLimitCounterStore(this IServiceCollection services)
        {
            services.AddTransient<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
        }
    }
}