using System.Collections.Generic;

namespace IdentityServerRequestTracker.RateLimit.Options
{
    public class ClientRateLimitingOptions
    {
        public List<RateLimitClientsRule> Rules { get; set; }
    }
}