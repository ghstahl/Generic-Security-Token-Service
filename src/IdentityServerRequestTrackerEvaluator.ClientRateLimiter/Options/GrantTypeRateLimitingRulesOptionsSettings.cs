using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Options
{
    public class GrantTypeRateLimitingRulesOptionsSettings
    {
        public bool Enabled { get; set; }
        public bool StackBlockedRequests { get; set; }
        public int HttpStatusCode { get; set; }
        public List<GeneralRules> GeneralRules { get; set; }
    }
}
 