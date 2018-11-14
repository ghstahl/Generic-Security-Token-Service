using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdentityServer4Extras.Models
{
    public partial class ApiResourceRecord
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scopeNameSpace")]
        public string ScopeNameSpace { get; set; }

        [JsonProperty("scopes")]
        public List<ApiResourceScope> Scopes { get; set; }
        [JsonProperty("secrets")]
        public List<string> Secrets { get; set; }
    }
}