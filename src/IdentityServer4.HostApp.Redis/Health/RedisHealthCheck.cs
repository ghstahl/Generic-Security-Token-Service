using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Contrib.RedisStore;
using IdentityServer4.Contrib.RedisStore.Cache;
using IdentityServer4.HostApp.Redis.Options;
using IdentityServer4.Services;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace IdentityServer4.HostApp.Redis.Health
{
    public class RedisHealthCheck : IHealthCheck
    {
        private const string CacheKey = "6a1b9283-a375-4487-babb-b00081f3a762";
        private RedisAppOptions _options;
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        public RedisHealthCheck(IOptions<RedisAppOptions> options, IServiceProvider serviceProvider,
            ILogger<RedisHealthCheck> logger):this(options.Value, serviceProvider, logger)
        {
            
        }
        public RedisHealthCheck(
            RedisAppOptions options, 
            IServiceProvider serviceProvider,
            ILogger<RedisHealthCheck> logger)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _logger = logger;
        }
        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_options.UseRedis)
            {
                return HealthCheckResult.Healthy("Redis is not configured for this app");
            }

            try
            {
                var cache =
                    _serviceProvider.GetService(typeof(ICache<string>)) as ICache<string>;

                string expected = "test_value";
                await cache.SetAsync(CacheKey, expected, TimeSpan.FromSeconds(1));
                var actual = await cache.GetAsync(CacheKey);
                bool success = actual == expected;
                if (success)
                {
                    return HealthCheckResult.Healthy("redis cache is healthy");
                }
                return HealthCheckResult.Healthy("redis cache is unhealthy");
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy("Exception: redis cache is unhealthy");
            }
        }
    }
}