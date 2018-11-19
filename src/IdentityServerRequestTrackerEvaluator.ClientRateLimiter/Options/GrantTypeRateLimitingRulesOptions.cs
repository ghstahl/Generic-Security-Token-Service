using System.Collections.Generic;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Options
{
    public class Rule
    {
        public List<string> GrantTypes { get; set; }
        public List<string> ClientIds { get; set; }
        public GrantTypeRateLimitingRulesOptionsSettings Settings { get; set; }
    }
}