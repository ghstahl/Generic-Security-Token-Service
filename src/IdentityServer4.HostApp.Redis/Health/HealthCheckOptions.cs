using Newtonsoft.Json;

namespace IdentityServer4.HostApp.Health
{
    public class HealthCheckOptions
    {
        [JsonProperty("groups")]
        public HealthCheckGroup[] Groups { get; set; }
    }
}