using System;
using System.Collections.Generic;

namespace AspNetCoreRateLimit.Services
{
    public interface IClientRateLimitProcessor
    {
        bool IsWhitelisted(ClientRequestIdentity identity);
        IEnumerable<RateLimitRule> GetMatchingRules(ClientRequestIdentity identity);
        RateLimitCounter ProcessRequest(ClientRequestIdentity identity, RateLimitRule rule);
        string RetryAfterFrom(DateTime counterTimestamp, RateLimitRule rule);
        RateLimitHeaders GetRateLimitHeaders(ClientRequestIdentity identity, RateLimitRule rule);
    }
}