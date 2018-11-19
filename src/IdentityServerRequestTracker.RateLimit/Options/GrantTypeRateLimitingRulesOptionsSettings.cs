using System.Collections.Generic;

namespace IdentityServerRequestTracker.RateLimit.Options
{
    public class GrantTypeRateLimitingRulesOptionsSettings
    {
        public bool StackBlockedRequests { get; set; }
        public int HttpStatusCode { get; set; }
        public List<RateLimitRule> RateLimitRules { get; set; }
        /// <summary>
        /// Gets or sets a value that will be used as a formatter for the QuotaExceeded response message.
        /// If none specified the default will be: 
        /// API calls quota exceeded! maximum admitted {0} per {1}
        /// </summary>
        public string QuotaExceededMessage { get; set; }
        /// <summary>
        /// Disables X-Rate-Limit and Rety-After headers
        /// </summary>
        public bool DisableRateLimitHeaders { get; set; }
    }
}
 