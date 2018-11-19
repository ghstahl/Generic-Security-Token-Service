using System.Collections.Generic;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Options
{
    public class ClientRateLimitingOptions
    {
        public List<Rule> Rules { get; set; }
    }
}