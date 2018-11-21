using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerRequestTracker;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.RateLimit.Options;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdentityServerRequestTracker.RateLimit.Services
{
    public class ClientRateLimiterRequestTrackerResult : IRequestTrackerResult
    {
        private IServiceProvider _serviceProvider;
        private IClientRateLimitProcessor _processor;

        public RequestTrackerEvaluatorDirective Directive { get; set; }
        public ClientRequestIdentity ClientRequestIdentity { get; set; }
        public IdentityServerRequestRecord IdentityServerRequestRecord { get; set; }
        public RateLimitRule Rule { get; set; }
        public string RetryAfter { get; set; }
        public RateLimitClientsRule RateLimitClientsRule { get; set; }

        public ClientRateLimiterRequestTrackerResult(
            IServiceProvider serviceProvider,
            IClientRateLimitProcessor processor)
        {

            _serviceProvider = serviceProvider;
            _processor = processor;
        }

        public async Task ProcessAsync(HttpContext httpContext)
        {
            var identity = new ClientRequestIdentity()
            {
                ClientId = IdentityServerRequestRecord.Client.ClientId,
                EndpointKey = IdentityServerRequestRecord.EndpointKey
            };
            if (Directive == RequestTrackerEvaluatorDirective.DenyRequest)
            {
                await ReturnQuotaExceededResponse(httpContext, Rule, RetryAfter);
            }
        }
        public virtual async Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            var message = string.IsNullOrEmpty(RateLimitClientsRule.Settings.QuotaExceededMessage) ? 
                $"API calls quota exceeded! maximum admitted {rule.Limit} per {rule.Period}." :
                RateLimitClientsRule.Settings.QuotaExceededMessage;

            if (!RateLimitClientsRule.Settings.DisableRateLimitHeaders)
            {
                httpContext.Response.Headers["Retry-After"] = retryAfter;
            }

            httpContext.Response.StatusCode = RateLimitClientsRule.Settings.HttpStatusCode;
            httpContext.Response.WriteAsync(message);
        }

    }
}