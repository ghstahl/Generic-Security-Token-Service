using Newtonsoft.Json;

namespace IdentityServer4.HostApp.Redis.Health
{
    public class HealthCheckGroup
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("urls")]
        public string[] Urls { get; set; }
    }
}