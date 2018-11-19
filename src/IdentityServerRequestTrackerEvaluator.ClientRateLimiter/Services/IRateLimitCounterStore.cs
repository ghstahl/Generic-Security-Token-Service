using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Services
{
    public interface IRateLimitCounterStore
    {
        bool Exists(string id);
        RateLimitCounter? Get(string id);
        void Remove(string id);
        void Set(string id, RateLimitCounter counter, TimeSpan expirationTime);
    }
}
