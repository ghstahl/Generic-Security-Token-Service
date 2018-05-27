using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace P7IdentityServer4.Cron
{
    internal class DataRefreshService : HostedService
    {
        private AzureKeyVaultTokenSigningServiceOptions _azureKeyVaultTokenSigningServiceOptions;
        private IKeyVaultCache _keyVaultCache;
        private const int Hours_6_in_seconds = 60 * 60 * 6;
        private const int Minutes_5_in_seconds = 60 * 5;
        public DataRefreshService(
            IOptions<AzureKeyVaultTokenSigningServiceOptions> keyVaultOptions,
            IKeyVaultCache keyVaultCache)
        {
            _azureKeyVaultTokenSigningServiceOptions = keyVaultOptions.Value;
            // 6 hours.
            if (_azureKeyVaultTokenSigningServiceOptions.DataRefreshCycleSeconds > Hours_6_in_seconds)
            {
                _azureKeyVaultTokenSigningServiceOptions.DataRefreshCycleSeconds = Hours_6_in_seconds;
            }
            // 5 minutes
            if (_azureKeyVaultTokenSigningServiceOptions.DataRefreshCycleSeconds < Minutes_5_in_seconds)
            {
                _azureKeyVaultTokenSigningServiceOptions.DataRefreshCycleSeconds = Minutes_5_in_seconds;
            }
           _keyVaultCache = keyVaultCache;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _keyVaultCache.RefreshCacheFromSourceAsync(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(_azureKeyVaultTokenSigningServiceOptions.DataRefreshCycleSeconds), cancellationToken);
            }
        }
    }
}