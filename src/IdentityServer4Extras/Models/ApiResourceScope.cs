using Newtonsoft.Json;

namespace IdentityServer4Extras.Models
{
    public partial class ApiResourceScope
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}