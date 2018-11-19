using System;
using System.Threading.Tasks;
using IdentityServerRequestTracker;
using IdentityServerRequestTracker.RateLimit.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerRequestTracker.RateLimit.Services
{
    internal class ClientRateLimiterRequestTrackerEvaluator : IIdentityServerRequestTrackerEvaluator
    {
        private IServiceProvider _serviceProvider;
        private IClientRateLimitProcessor _processor;
        private ILogger<ClientRateLimiterRequestTrackerEvaluator> _logger;

        public ClientRateLimiterRequestTrackerEvaluator(
            IServiceProvider serviceProvider,
            IClientRateLimitProcessor processor,
            ILogger<ClientRateLimiterRequestTrackerEvaluator> logger
            )
        {
            _serviceProvider = serviceProvider;
            _processor = processor;
            _logger = logger;
            Name = "client_rate_limiter";
        }
        public string Name { get; set; }
        public async Task<IRequestTrackerResult> EvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
           
            if (string.IsNullOrEmpty(requestRecord.ClientId))
            {
                // not for us
                return new AllowRequestTrackerResult();
            }

            var result = await EvaluateRateLimitRules(requestRecord);
            return result;
        }
        public virtual void LogBlockedRequest(
            HttpContext httpContext, 
            ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            _logger.LogInformation($"Request {httpContext.Request.Method}:{identity.EndpointKey} from ClientId {identity.ClientId} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.TotalRequests}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.");
        }

        async Task<IRequestTrackerResult> EvaluateRateLimitRules(IdentityServerRequestRecord requestRecord)
        {
            var identity = new ClientRequestIdentity()
            {
                ClientId = requestRecord.ClientId,
                EndpointKey = requestRecord.EndpointKey
            };
            var rateLimitClientsRule = _processor.GetRateLimitClientsRule(identity);
            if (rateLimitClientsRule != null)
            {
                var result = _serviceProvider.GetService<ClientRateLimiterRequestTrackerResult>();
                result.RateLimitClientsRule = rateLimitClientsRule;
                foreach (var rule in rateLimitClientsRule.Settings.RateLimitRules)
                {
                    if (rule.Limit > 0)
                    {
                        // increment counter
                        var counter = _processor.ProcessRequest(identity, rule);
                        // check if key expired
                        if (counter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                        {
                            continue;
                        } // check if limit is reached

                        if (counter.TotalRequests > rule.Limit)
                        {
                            //compute retry after value
                            var retryAfter = _processor.RetryAfterFrom(counter.Timestamp, rule);

                            // log blocked request
                            LogBlockedRequest(requestRecord.HttpContext, identity, counter, rule);

                            // break execution
                         
                            result.Directive = RequestTrackerEvaluatorDirective.DenyRequest;
                            result.IdentityServerRequestRecord = requestRecord;
                            result.Rule = rule;
                            result.RetryAfter = retryAfter;
                            result.RateLimitClientsRule = rateLimitClientsRule;


                            return result;
                        }
                    }
                    else
                    {
                        // process request count
                        var counter = _processor.ProcessRequest(identity, rule);

                        // log blocked request
                        LogBlockedRequest(requestRecord.HttpContext, identity, counter, rule);

                        // break execution (Int32 max used to represent infinity)
                        // break execution
                      
                        result.Directive = RequestTrackerEvaluatorDirective.DenyRequest;
                        result.IdentityServerRequestRecord = requestRecord;
                        result.Rule = rule;
                        result.RetryAfter = Int32.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        return result;
                    }
                }
            }


            var allowResult = _serviceProvider.GetService<ClientRateLimiterRequestTrackerResult>();
            allowResult.Directive = RequestTrackerEvaluatorDirective.AllowRequest;
            return allowResult;

        }
    }
}