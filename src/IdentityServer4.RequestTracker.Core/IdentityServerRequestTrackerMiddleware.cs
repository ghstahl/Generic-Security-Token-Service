using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4RequestTracker
{
    public class IdentityServerRequestTrackerMiddleware
    {
        private readonly SecretParser _parser;
        private readonly IDiscoveryResponseGenerator _responseGenerator;
        private readonly RequestDelegate _next;
        private readonly ILogger<IdentityServerRequestTrackerMiddleware> _logger;
 
        private Dictionary<string, string> _endpointDictionary;
        private IEnumerable<IIdentityServerRequestTrackerEvaluator> _evaluators;

        public IdentityServerRequestTrackerMiddleware(
            SecretParser parser, 
            IDiscoveryResponseGenerator responseGenerator, 
            IEnumerable<IIdentityServerRequestTrackerEvaluator> evaluators,
            RequestDelegate next,
            ILogger<IdentityServerRequestTrackerMiddleware> logger)
        {
            _parser = parser;
            _responseGenerator = responseGenerator;
            _evaluators = evaluators;
            _next = next;
            _logger = logger;
        }

        private async Task<Dictionary<string, string>> FetchDiscoveryData(HttpContext httpContext)
        {
            if (_endpointDictionary == null)
            {
                var baseUrl = httpContext.GetIdentityServerBaseUrl().EnsureTrailingSlash();
                var issuerUri = httpContext.GetIdentityServerIssuerUri();
                // generate response
                _logger.LogTrace("Calling into discovery response generator: {type}", _responseGenerator.GetType().FullName);
                var response = await _responseGenerator.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);

                var issuer = response["issuer"] as string;

                _endpointDictionary = new Dictionary<string, string>
                {
                    {"discovery", "/.well-known/openid-configuration"},
                    {"jwks_uri", (response["jwks_uri"] as string).Substring(issuer.Length)},
                    {
                        "authorization_endpoint",
                        (response["authorization_endpoint"] as string).Substring(issuer.Length)
                    },
                    {"token_endpoint", (response["token_endpoint"] as string).Substring(issuer.Length)},
                    {"userinfo_endpoint", (response["userinfo_endpoint"] as string).Substring(issuer.Length)},
                    {
                        "end_session_endpoint",
                        (response["end_session_endpoint"] as string).Substring(issuer.Length)
                    },
                    {
                        "check_session_iframe",
                        (response["check_session_iframe"] as string).Substring(issuer.Length)
                    },
                    {"revocation_endpoint", (response["revocation_endpoint"] as string).Substring(issuer.Length)},
                    {
                        "introspection_endpoint",
                        (response["introspection_endpoint"] as string).Substring(issuer.Length)
                    } 
                };
            }
            return _endpointDictionary;
        }
        

        public async Task Invoke(HttpContext httpContext)
        {
            // start tracking
            await FetchDiscoveryData(httpContext);
            var endpointKey = (from item in _endpointDictionary
                where item.Value == httpContext.Request.Path.Value
                select item.Key).FirstOrDefault();
            if (endpointKey != null)
            {
                _logger.LogInformation($"endpointKey={endpointKey},path={httpContext.Request.Path}");
                var requestRecord = new IdentityServerRequestRecord
                {
                    HttpContext = httpContext,
                    EndpointKey = endpointKey
                };
                if (httpContext.Request.Method == "POST")
                {
                    var parsedSecret = await _parser.ParseAsync(httpContext);
                    if (parsedSecret != null)
                    {
                        // this is a request for a token
                        requestRecord.ClientId = parsedSecret.Id;
                    }
                }

                foreach (var evaluator in _evaluators)
                {
                    var directive = await ProcessEvaluatorAsync(evaluator, requestRecord);
                    if (directive == RequestTrackerEvaluatorDirective.DenyRequest)
                    {
                        return; // do not continue to the real IdentityServer4 middleware.
                    }
                }
            }
           
            await _next(httpContext);
        }

        private async Task<RequestTrackerEvaluatorDirective> ProcessEvaluatorAsync(
            IIdentityServerRequestTrackerEvaluator evaluator, 
            IdentityServerRequestRecord requestRecord)
        {
            string name = "";
            RequestTrackerEvaluatorDirective directive = RequestTrackerEvaluatorDirective.AllowRequest;
            try
            {
                name = evaluator.Name;
                var result = await evaluator.EvaluateAsync(requestRecord);
                await result.ProcessAsync(requestRecord.HttpContext);
                directive = result.Directive;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,$"EvaluatorProcessed Exception,message={e.Message} ");
            }
            _logger.LogInformation($"EvaluatorProcessed,name={name},directive={directive}");
            return directive;
        }
    }
}