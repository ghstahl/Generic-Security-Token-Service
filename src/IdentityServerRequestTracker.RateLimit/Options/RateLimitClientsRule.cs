using System.Collections.Generic;

namespace IdentityServerRequestTracker.RateLimit.Options
{
    public class RateLimitClientsRule
    {
        public bool Enabled { get; set; }
        public List<string> GrantTypes { get; set; }
        public List<string> ClientIds { get; set; }
        public GrantTypeRateLimitingRulesOptionsSettings Settings { get; set; }
    }
}