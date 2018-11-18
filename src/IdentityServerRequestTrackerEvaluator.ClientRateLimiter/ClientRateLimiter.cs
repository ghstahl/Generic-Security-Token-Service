using System;
using System.Threading.Tasks;
using IdentityServer4RequestTracker;

namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter
{
    internal class ClientRateLimiter: IIdentityServerRequestTrackerEvaluator
    {
        public ClientRateLimiter()
        {
            Name = "client_rate_limiter";
        }
        public string Name { get; set; }
        public async Task<RequestTrackerEvaluatorDirective> EvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
            return RequestTrackerEvaluatorDirective.AllowRequest;
        }
    }
}
