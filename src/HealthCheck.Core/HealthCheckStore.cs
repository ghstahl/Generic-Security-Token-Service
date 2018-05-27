using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public class HealthCheckStore : IHealthCheckStore
    {
        SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private ConcurrentDictionary<string,HealthRecord> HealthRecords { get; set; }
        private AggregateHealthRecord _aggregateRecord { get; set; }

        public HealthCheckStore()
        {
            HealthRecords = new ConcurrentDictionary<string, HealthRecord>();
            _aggregateRecord = AggregateHealthRecord.Unhealthy(new []{""});
        }
 
        public async Task SetHealthAsync(string key, HealthRecord healthRecord)
        {
            HealthRecords[key] = healthRecord;
            _aggregateRecord = await InternalIsHealthyAsync();
        }

        public void SetHealth(string key, HealthRecord healthRecord)
        {
            SetHealthAsync(key, healthRecord).GetAwaiter().GetResult();
        }

        public async Task<HealthRecord> GetHealthAsync(string key)
        {
            HealthRecord value;
            if (!HealthRecords.TryGetValue(key, out value))
            {
                return null;
            }
            return value;
        }

        public HealthRecord GetHealth(string key)
        {
            return GetHealthAsync(key).GetAwaiter().GetResult();
        }

        public async Task<AggregateHealthRecord> InternalIsHealthyAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                
                if (HealthRecords.IsEmpty)
                    return null;
                var unHealtyRecords = new List<string>();
               
                foreach (var item in HealthRecords)
                {
                    if (item.Value.Healty == false)
                    {
                        unHealtyRecords.Add(item.Key);
                    }
                }
                if (unHealtyRecords.Any())
                {
                    return AggregateHealthRecord.Unhealthy(unHealtyRecords.ToArray());
                }
                return AggregateHealthRecord.Healthy();
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
        }

        public async Task<AggregateHealthRecord> IsHealthyAsync()
        {
            return _aggregateRecord;
        }

        public AggregateHealthRecord IsHealthy()
        {
            return _aggregateRecord;
        }
    }
}