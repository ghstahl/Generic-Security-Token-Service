using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.HostApp.Redis.Options;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7IdentityServer4;

namespace IdentityServer4.HostApp.Redis.Health
{
    public class KeyVaultHealthCheck : IHealthCheck
    {
        private KeyVaultAppOptions _options;
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        public KeyVaultHealthCheck(
            IOptions<KeyVaultAppOptions> options, 
            IServiceProvider serviceProvider,
            ILogger<KeyVaultHealthCheck> logger) : this(options.Value, serviceProvider, logger)
        {

        }
        public KeyVaultHealthCheck(
            KeyVaultAppOptions options,
            IServiceProvider serviceProvider,
            ILogger<KeyVaultHealthCheck> logger)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _logger = logger;
        }
        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_options.UseKeyVault)
            {
                return HealthCheckResult.Healthy("KeyVault is not configured for this app");
            }

            try
            {
                var provider =
                    _serviceProvider.GetService(typeof(IPublicKeyProvider)) as IPublicKeyProvider;
                var keyBundle = await provider.GetKeyBundleAsync();

                var success = keyBundle != null;

                if (success)
                {
                    return HealthCheckResult.Healthy("KeyVault is healthy");
                }
                return HealthCheckResult.Healthy("KeyVault is unhealthy");
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy("Exception: redis cache is unhealthy");
            }
        }
    }
}