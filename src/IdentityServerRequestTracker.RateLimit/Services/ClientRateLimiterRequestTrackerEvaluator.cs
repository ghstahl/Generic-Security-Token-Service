using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServerRequestTracker;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.RateLimit.Options;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

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

        public async Task<IRequestTrackerResult> PreEvaluateAsync(IdentityServerRequestRecord requestRecord)
        {

            if (requestRecord.Client == null)
            {
                // not for us
                return null;
            }

            var result = await PreEvaluateRateLimitRules(requestRecord);
            return result;
        }

         

        public async Task<IRequestTrackerResult> PostEvaluateAsync(IdentityServerRequestRecord requestRecord, bool error)
        {
            if (requestRecord.Client == null)
            {
                // not for us
                return null;
            }

            var result = await PostEvaluateRateLimitRules(requestRecord, error);
            return result;
        }


        public virtual void LogBlockedRequest(
            HttpContext httpContext,
            ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            _logger.LogInformation(
                $"Request {httpContext.Request.Method}:{identity.EndpointKey} from ClientId {identity.ClientId} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.TotalRequests}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.");
        }

        async Task<IRequestTrackerResult> PreEvaluateRateLimitRules(IdentityServerRequestRecord requestRecord)
        {
            // ONLY CHECK THE COUNT and Deny.  DO NOT MUTATE any data
            var identity = new ClientRequestIdentity()
            {
                ClientId = requestRecord.Client.ClientId,
                EndpointKey = requestRecord.EndpointKey
            };
            var rateLimitClientsRule = _processor.GetRateLimitClientsRule(identity);
            if (rateLimitClientsRule == null)
            {
                // no rules for us to evaluate
                return null;
            }

            var result = _serviceProvider.GetService<ClientRateLimiterRequestTrackerResult>();
            result.RateLimitClientsRule = rateLimitClientsRule;
            result.IdentityServerRequestRecord = requestRecord;
            result.Directive = RequestTrackerEvaluatorDirective.AllowRequest;
            foreach (var rule in rateLimitClientsRule.Settings.RateLimitRules)
            {
                if (rule.Limit > 0)
                {
                    // get the current counter
                    var counter = _processor.GetCurrentRateLimitCounter(identity, rule);
                    // check if key expired
                    if (counter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                    {
                        continue;
                    } // check if limit is reached

                    if (counter.TotalRequests >= rule.Limit)
                    {
                        //compute retry after value
                        var retryAfter = _processor.RetryAfterFrom(counter.Timestamp, rule);

                        // log blocked request
                        LogBlockedRequest(requestRecord.HttpContext, identity, counter, rule);

                        // break execution

                        result.Directive = RequestTrackerEvaluatorDirective.DenyRequest;
                        result.Rule = rule;
                        result.RetryAfter = retryAfter;

                        return result;
                    }
                }
                else
                {
                    // process request count
                    var counter = _processor.GetCurrentRateLimitCounter(identity, rule);

                    // log blocked request
                    LogBlockedRequest(requestRecord.HttpContext, identity, counter, rule);

                    // break execution (Int32 max used to represent infinity)
                    // break execution

                    result.Directive = RequestTrackerEvaluatorDirective.DenyRequest;
                    result.Rule = rule;
                    result.RetryAfter = Int32.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    return result;
                }
            }

            result.Rule = rateLimitClientsRule.Settings.RateLimitRules
                .OrderByDescending(x => x.PeriodTimespan.Value).First();

            return result;

        }

        async Task<IRequestTrackerResult> PostEvaluateRateLimitRules(IdentityServerRequestRecord requestRecord,bool error)
        {
            if (!error)
            {
                // only increment if successful 
                // Increment the counters on the way out.  Allow the request to finish
                var identity = new ClientRequestIdentity()
                {
                    ClientId = requestRecord.Client.ClientId,
                    EndpointKey = requestRecord.EndpointKey
                };
                var rateLimitClientsRule = _processor.GetRateLimitClientsRule(identity);
                if (rateLimitClientsRule != null)
                {
                    foreach (var rule in rateLimitClientsRule.Settings.RateLimitRules)
                    {
                        if (rule.Limit > 0)
                        {
                            // increment counter
                            var counter = _processor.ProcessRequest(identity, rule);
                        }
                    }

                    //set X-Rate-Limit headers for the longest period
                    if (rateLimitClientsRule.Settings.RateLimitRules.Any()
                        && !rateLimitClientsRule.Settings.DisableRateLimitHeaders)
                    {
                        var rule = rateLimitClientsRule.Settings.RateLimitRules
                            .OrderByDescending(x => x.PeriodTimespan.Value).First();
                        var headers = _processor.GetRateLimitHeaders(identity, rule);
                        headers.Context = requestRecord.HttpContext;
                        requestRecord.HttpContext.Response.OnStarting(SetRateLimitHeaders, state: headers);
                    }
                }
            }
            return null;
        }

        private Task SetRateLimitHeaders(object state)
        {
            var headers = (RateLimitHeaders) state;
            if (headers != null)
            {
                headers.Context.Response.Headers["X-Rate-Limit-Limit"] = headers.Limit;
                headers.Context.Response.Headers["X-Rate-Limit-Remaining"] = headers.Remaining;
                headers.Context.Response.Headers["X-Rate-Limit-Reset"] = headers.Reset;
            }

            return Task.CompletedTask;
        }
    }
}