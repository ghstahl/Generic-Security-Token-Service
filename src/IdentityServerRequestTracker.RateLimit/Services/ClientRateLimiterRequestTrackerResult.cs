using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerRequestTracker;
using IdentityServerRequestTracker.RateLimit.Options;
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
                ClientId = IdentityServerRequestRecord.ClientId,
                EndpointKey = IdentityServerRequestRecord.EndpointKey
            };
            if (Directive == RequestTrackerEvaluatorDirective.DenyRequest)
            {
                await ReturnQuotaExceededResponse(httpContext, Rule, RetryAfter);
            }
            else
            {
                //set X-Rate-Limit headers for the longest period
                if (RateLimitClientsRule.Settings.RateLimitRules.Any()
                    && !RateLimitClientsRule.Settings.DisableRateLimitHeaders)
                {
                    var rule = RateLimitClientsRule.Settings.RateLimitRules.OrderByDescending(x => x.PeriodTimespan.Value).First();
                    var headers = _processor.GetRateLimitHeaders(identity, rule);
                    headers.Context = httpContext;

                    httpContext.Response.OnStarting(SetRateLimitHeaders, state: headers);
                }
            }
        }
        private Task SetRateLimitHeaders(object rateLimitHeaders)
        {
            var headers = (RateLimitHeaders)rateLimitHeaders;

            headers.Context.Response.Headers["X-Rate-Limit-Limit"] = headers.Limit;
            headers.Context.Response.Headers["X-Rate-Limit-Remaining"] = headers.Remaining;
            headers.Context.Response.Headers["X-Rate-Limit-Reset"] = headers.Reset;

            return Task.CompletedTask;
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