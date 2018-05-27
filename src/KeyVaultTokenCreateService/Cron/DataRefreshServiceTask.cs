using System;
using System.Threading;
using System.Threading.Tasks;
using HealthCheck.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7.Core.Scheduler.Scheduling;

namespace P7IdentityServer4.Cron
{
    public class DataRefreshServiceTask : IScheduledTask
    {
        private AzureKeyVaultTokenSigningServiceOptions _azureKeyVaultTokenSigningServiceOptions;
        private IKeyVaultCache _keyVaultCache;
        private IHealthCheckStore _healthCheckStore;
        private ILogger _logger;
        private const string Every6Hours = "0 */6 * * *"; //https://crontab.guru/every-6-hours
        private const string Every5Minutes = "*/5 * * * *"; //https://crontab.guru/every-5-minutes

        public DataRefreshServiceTask(
            IOptions<AzureKeyVaultTokenSigningServiceOptions> keyVaultOptions,
            IHealthCheckStore healthCheckStore,
            ILogger<KeyVaultCache> logger,
            IKeyVaultCache keyVaultCache)
        {
            _logger = logger;
            _healthCheckStore = healthCheckStore;
            _azureKeyVaultTokenSigningServiceOptions = keyVaultOptions.Value;
            // 6 hours.
            if (string.IsNullOrWhiteSpace(_azureKeyVaultTokenSigningServiceOptions.CronScheduleDataRefresh))
            {
                _azureKeyVaultTokenSigningServiceOptions.CronScheduleDataRefresh = Every5Minutes;
            }
            _keyVaultCache = keyVaultCache;
            Schedule = _azureKeyVaultTokenSigningServiceOptions.CronScheduleDataRefresh;
        }

        public string Schedule { get; }
        

        public async Task Invoke(CancellationToken cancellationToken)
        {
            var key = "keyvault-data-refresh";
            var currentHealth = await _healthCheckStore.GetHealthAsync(key);
            if (currentHealth == null)
            {
                currentHealth = new HealthRecord()
                {
                    Healty = false,
                    State = null
                };
                await _healthCheckStore.SetHealthAsync(key, currentHealth);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                bool success = false;
                try
                {
                    await _keyVaultCache.RefreshCacheFromSourceAsync(cancellationToken);
                    success = true;
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e,"Cron job to pull keyvault data failure!");
                }
                finally
                {
                    currentHealth = new HealthRecord()
                    {
                        Healty = success,
                        State = null
                    };
                    await _healthCheckStore.SetHealthAsync(key, currentHealth);
                }
            }
        }
    }
}