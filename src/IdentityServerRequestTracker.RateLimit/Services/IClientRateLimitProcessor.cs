using System;
using System.Collections.Generic;
using IdentityServerRequestTracker.RateLimit.Options;

namespace IdentityServerRequestTracker.RateLimit.Services
{
    public interface IClientRateLimitProcessor
    {

        RateLimitClientsRule GetRateLimitClientsRule(ClientRequestIdentity identity);
        RateLimitCounter ProcessRequest(ClientRequestIdentity identity, RateLimitRule rule);
        string RetryAfterFrom(DateTime counterTimestamp, RateLimitRule rule);
        RateLimitHeaders GetRateLimitHeaders(ClientRequestIdentity identity, RateLimitRule rule);
    }
}