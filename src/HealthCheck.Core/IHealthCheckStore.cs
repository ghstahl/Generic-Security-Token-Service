using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public class HealthRecord  
    {
        public bool Healty { get; set; }
        public object State { get; set; }
    }

    public class AggregateHealthRecord
    {
        public static AggregateHealthRecord Unhealthy(string[] unHealthyKeys)
            => new AggregateHealthRecord(false, unHealthyKeys);
        public static AggregateHealthRecord Healthy()
            => new AggregateHealthRecord(true,null);

        private  AggregateHealthRecord(bool health, string[] unHealtyKeys)
        {
            Health = health;
            UnHealthyKeys = unHealtyKeys;
        }
        public bool Health { get; private set; }
        public string[] UnHealthyKeys { get; private set; }
    }

    public interface IHealthCheckStore
    {
        Task SetHealthAsync(string key, HealthRecord healthRecord);
        void SetHealth(string key, HealthRecord healthRecord);
        Task<HealthRecord> GetHealthAsync(string key);
        HealthRecord GetHealth(string key);

        Task<AggregateHealthRecord> IsHealthyAsync();
        AggregateHealthRecord IsHealthy();
    }
}