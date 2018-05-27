using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.HealthChecks;

namespace HealthCheck.Core
{
    public class AggregateHealthCheck : IHealthCheck
    {
        private readonly IHealthCheckStore _store;

        public AggregateHealthCheck(IHealthCheckStore store)
        {
            _store = store;
        }

        public async ValueTask<IHealthCheckResult> CheckAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IHealthCheckResult result = null;
            if (_store == null)
            {
                result = HealthCheckResult.Unhealthy("IHealthCheckStore is null, DI did not work");
            }
            var health = await _store.IsHealthyAsync();
            if (health.Health == false)
            {
                var unhealthyKeys = string.Join(".", health.UnHealthyKeys);
                result = HealthCheckResult.Unhealthy($"the following aggregate keys are unhealthy:[{unhealthyKeys}]");
            }
            else
            {
                result = HealthCheckResult.Healthy("Everything in Aggregate is healthy");
            }
            return result;

        }
    }
}
