using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using IdentityServer4.Events;
using IdentityServer4.Hosting;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4.HostApp.RateLimiting
{
    public enum ClientRateLimiterDirective
    {
        Deny = 0,
        Allow,
        SetRateLimitHeaders
    }
    public interface IClientRateLimiterResult
    {
        ClientRateLimiterDirective Directive { get; }
        RateLimitRule Rule { get; }
        string retryAfter { get; }
        RateLimitHeaders Headers { get; }
    }


    public class ClientRateLimiterResult : IClientRateLimiterResult
    {
        public ClientRateLimiterDirective Directive { get; set; }
        public RateLimitRule Rule { get; set; }
        public string retryAfter { get; set; }
        public RateLimitHeaders Headers { get; set; }
    }
    public interface IClientRateLimiterHandler
    {
        Task<IClientRateLimiterResult> ProcessAsync(HttpContext context);
    }

    public class ClientRateLimiterHandler : IClientRateLimiterHandler
    {
        private ILogger<ClientRateLimiterHandler> _logger;
        private IClientStore _clients;
        private SecretParser _parser;
        private IRateLimitCounterStore _counterStore;
        private IClientPolicyStore _policyStore;
        private IEventService _events;
        private ClientRateLimitOptions _options;
        private ClientRateLimitProcessor _processor;

        public ClientRateLimiterHandler(
            IOptions<ClientRateLimitOptions> options,
            IRateLimitCounterStore counterStore,
            IClientPolicyStore policyStore,
            IClientStore clients,
            SecretParser parser,
            IEventService events,
            ILogger<ClientRateLimiterHandler> logger)
        {
            _options = options.Value;
            _counterStore = counterStore;
            _policyStore = policyStore;
            _clients = clients;
            _parser = parser;
            _events = events;
            _logger = logger;
            _processor = new ClientRateLimitProcessor(_options, counterStore, policyStore);
        }

        public async Task<IClientRateLimiterResult> ProcessAsync(HttpContext httpContext)
        {
            var result = new ClientRateLimiterResult
            {
                Directive = ClientRateLimiterDirective.Deny
            };
            var parsedSecret = await _parser.ParseAsync(httpContext);
            if (parsedSecret == null)
            {
                return result;
            }

            // load client
            var client = await _clients.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (client == null || IsWhitelisted(client.ClientId))
            {
                return result;
            }
            var identity = new ClientRequestIdentity
            {
                Path = httpContext.Request.Path.ToString().ToLowerInvariant(),
                HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
                ClientId = client.ClientId
            };
            var rules = _processor.GetMatchingRules(identity);

            foreach (var rule in rules)
            {
                if (rule.Limit > 0)
                {
                    // increment counter
                    var counter = _processor.ProcessRequest(identity, rule);

                    // check if key expired
                    if (counter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                    {
                        continue;
                    }

                    // check if limit is reached
                    if (counter.TotalRequests > rule.Limit)
                    {
                        //compute retry after value
                        var retryAfter = _processor.RetryAfterFrom(counter.Timestamp, rule);

                        // log blocked request
                        LogBlockedRequest(httpContext, identity, counter, rule);
                        result.Rule = rule;
                        result.Directive = ClientRateLimiterDirective.Deny;
                        result.retryAfter = retryAfter;
                        // break execution

                        return result;
                    }
                }
                // if limit is zero or less, block the request.
                else
                {
                    // process request count
                    var counter = _processor.ProcessRequest(identity, rule);

                    // log blocked request
                    LogBlockedRequest(httpContext, identity, counter, rule);

                    result.Rule = rule;
                    result.Directive = ClientRateLimiterDirective.Deny;
                    result.retryAfter = Int32.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    // break execution (Int32 max used to represent infinity)
                    return result;
                }
            }

            //set X-Rate-Limit headers for the longest period
            if (rules.Any() && !_options.DisableRateLimitHeaders)
            {
                var rule = rules.OrderByDescending(x => x.PeriodTimespan.Value).First();
                var headers = _processor.GetRateLimitHeaders(identity, rule);
                headers.Context = httpContext;
                result.Directive = ClientRateLimiterDirective.SetRateLimitHeaders;
                result.Headers = headers;
                return result;
            }

            result.Directive = ClientRateLimiterDirective.Allow;
            return result;
        }
        public virtual void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            _logger.LogInformation($"Request {identity.HttpVerb}:{identity.Path} from ClientId {identity.ClientId} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.TotalRequests}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.");
        }
        public bool IsWhitelisted(string clientId)
        {
            if (_options.ClientWhitelist != null && _options.ClientWhitelist.Contains(clientId))
            {
                return true;
            }
            return false;
        }
        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ClientAuthenticationFailureEvent(clientId, message));
        }
    }
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class IdentityServerClientMiddleware
    {
        private readonly RequestDelegate _next;
        private IClientStore _clients;
        private SecretParser _parser;

        private ILogger<IdentityServerMiddleware> _logger;
        private IClientRateLimiterHandler _clientRateLimiterHandler;
        private ClientRateLimitOptions _options;


        public IdentityServerClientMiddleware(
            RequestDelegate next,
            IOptions<ClientRateLimitOptions> options,
            IClientRateLimiterHandler clientRateLimiterHandler,
            ILogger<IdentityServerMiddleware> logger)
        {
            _next = next;
            _options = options.Value;
            _clientRateLimiterHandler = clientRateLimiterHandler;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, IEndpointRouter router, IEventService events)
        { 
            // check if rate limiting is enabled
            if (_options == null)
            {
                await _next.Invoke(httpContext);
                return;
            }

            try
            {
                if (HttpMethods.IsPost(httpContext.Request.Method) && httpContext.Request.HasFormContentType)
                {
                    var endpoint = router.Find(httpContext);
                    if (endpoint != null)
                    {
                        var result = await _clientRateLimiterHandler.ProcessAsync(httpContext);
                        switch (result.Directive)
                        {
                            case ClientRateLimiterDirective.Deny:
                                await ReturnQuotaExceededResponse(httpContext, result.Rule, result.retryAfter);
                                return;
                                break;
                            case ClientRateLimiterDirective.SetRateLimitHeaders:
                                httpContext.Response.OnStarting(SetRateLimitHeaders, state: result.Headers);
                                break;
                            case ClientRateLimiterDirective.Allow:
                            default:
                                break;
                        }
                      
                    }
                }

            }
            catch (Exception ex)
            {
                await events.RaiseAsync(new UnhandledExceptionEvent(ex));
                _logger.LogCritical(ex, "Unhandled exception: {exception}", ex.Message);
                throw;
            }

            await _next(httpContext);
        }
        public async Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            var message = string.IsNullOrEmpty(_options.QuotaExceededMessage) ? $"API calls quota exceeded! maximum admitted {rule.Limit} per {rule.Period}." : _options.QuotaExceededMessage;

            if (!_options.DisableRateLimitHeaders)
            {
                httpContext.Response.Headers["Retry-After"] = retryAfter;
            }

            httpContext.Response.StatusCode = _options.HttpStatusCode;
            httpContext.Response.WriteAsync(message);
        }
        private Task SetRateLimitHeaders(object rateLimitHeaders)
        {
            var headers = (RateLimitHeaders)rateLimitHeaders;

            headers.Context.Response.Headers["X-Rate-Limit-Limit"] = headers.Limit;
            headers.Context.Response.Headers["X-Rate-Limit-Remaining"] = headers.Remaining;
            headers.Context.Response.Headers["X-Rate-Limit-Reset"] = headers.Reset;

            return Task.CompletedTask;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IdentityServerClientMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdentityServerClientMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdentityServerClientMiddleware>();
        }
    }
}
