using Newtonsoft.Json;

namespace IdentityServer4.HostApp.Redis.Health
{
    public class HealthCheckOptions
    {
        [JsonProperty("groups")]
        public HealthCheckGroup[] Groups { get; set; }
    }
}