using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using P7.Core.Cache;

namespace IdentityServerRequestTracker.Middleware
{
    public class PreIdentityServerMiddleware
    {
        private  string PathRootUrl { get; set; }
        private readonly IDiscoveryResponseGenerator _responseGenerator;
        private readonly RequestDelegate _next;
        private readonly ILogger<PreIdentityServerMiddleware> _logger;
 
        private Dictionary<string, string> _endpointDictionary;
        private IEnumerable<IIdentityServerRequestTrackerEvaluator> _evaluators;
        private IClientSecretValidator _clientValidator;
        private ISingletonAutoObjectCache<PreIdentityServerMiddleware, Dictionary<string, object>> _objectCache;
        private IConfiguration _configuration;
        List<string> KnownEndpointPaths { get; set; }

        public PreIdentityServerMiddleware(
            ISingletonAutoObjectCache<PreIdentityServerMiddleware, Dictionary<string, object>> objectCache,
            IConfiguration configuration,
            IClientSecretValidator clientValidator,
            IDiscoveryResponseGenerator responseGenerator,
            IEnumerable<IIdentityServerRequestTrackerEvaluator> evaluators,
            RequestDelegate next,
            ILogger<PreIdentityServerMiddleware> logger)
        {
            _objectCache = objectCache;
            _configuration = configuration;

            _clientValidator = clientValidator;
            _responseGenerator = responseGenerator;
            _evaluators = evaluators;
            _next = next;
            _logger = logger;

            KnownEndpointPaths = new List<string>()
            {
                "/.well-known/openid-configuration",
                "/.well-known/openid-configuration/jwks",
                "/connect/authorize",
                "/connect/token",
                "/connect/userinfo",
                "/connect/endsession",
                "/connect/checksession",
                "/connect/revocation",
                "/connect/introspect",
                "/connect/deviceauthorization"
            };
            PathRootUrl = _configuration["IdentityServerPublicFacingUri"];
            if (!string.IsNullOrEmpty(PathRootUrl))
            {
                PathRootUrl = PathRootUrl.TrimEnd('/');
                PathRootUrl = $"/{PathRootUrl}";
            }
        }

        private async Task<Dictionary<string, string>> FetchDiscoveryData(HttpContext httpContext)
        {
            if (_endpointDictionary == null)
            {
                var baseUrl = httpContext.GetIdentityServerBaseUrl().EnsureTrailingSlash();
                var issuerUri = httpContext.GetIdentityServerIssuerUri();
                var index = issuerUri.IndexOf("://") + 3;
                var runningUri = issuerUri.Substring(index);
                index = runningUri.IndexOf('/');
                var baseUrlSegment = index >=0?runningUri.Substring(index):"";
                // generate response
                _logger.LogTrace("Calling into discovery response generator: {type}",
                    _responseGenerator.GetType().FullName);
                var response = await _responseGenerator.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);

                var issuer = response["issuer"] as string;

                _endpointDictionary = new Dictionary<string, string>
                {
                    {
                        "discovery",
                        $"{baseUrlSegment}/.well-known/openid-configuration"
                    },
                    {
                        "jwks_uri",
                        $"{baseUrlSegment}{(response["jwks_uri"] as string).Substring(issuer.Length)}"
                    },
                    {
                        "authorization_endpoint",
                        $"{baseUrlSegment}{(response["authorization_endpoint"] as string).Substring(issuer.Length)}"
                    },
                    {
                        "token_endpoint",
                        $"{baseUrlSegment}{(response["token_endpoint"] as string).Substring(issuer.Length)}"
                    },
                    {
                        "userinfo_endpoint",
                        $"{baseUrlSegment}{(response["userinfo_endpoint"] as string).Substring(issuer.Length)}"
                    },
                    {
                        "end_session_endpoint",
                        $"{baseUrlSegment}{(response["end_session_endpoint"] as string).Substring(issuer.Length)}"
                    },
                    {
                        "check_session_iframe",
                        $"{baseUrlSegment}{(response["check_session_iframe"] as string).Substring(issuer.Length)}"
                    },

                    {
                        "revocation_endpoint",
                        $"{baseUrlSegment}{(response["revocation_endpoint"] as string).Substring(issuer.Length)}"
                    },
                    {
                        "introspection_endpoint",
                        $"{baseUrlSegment}{(response["introspection_endpoint"] as string).Substring(issuer.Length)}"
                    }

                };
            }

            return _endpointDictionary;
        }


        public async Task Invoke(HttpContext httpContext, IScopedStorage scopedStorage)
        {
            if (!string.IsNullOrEmpty(PathRootUrl))
            {
                FixUpPath(httpContext);
            }

            // start tracking
            await FetchDiscoveryData(httpContext);
            var endpointKey = (from item in _endpointDictionary
                where item.Value == httpContext.Request.Path.Value
                select item.Key).FirstOrDefault();
            if (endpointKey == null)
            {
                // not for us
                await _next(httpContext);
                return;
            }

            _logger.LogInformation($"endpointKey={endpointKey},path={httpContext.Request.Path}");
            var requestRecord = new IdentityServerRequestRecord
            {
                HttpContext = httpContext,
                EndpointKey = endpointKey
            };
            // validate HTTP for clients
            if (HttpMethods.IsPost(httpContext.Request.Method) && httpContext.Request.HasFormContentType)
            {
                // validate client
                var clientResult = await _clientValidator.ValidateAsync(httpContext);
                if (!clientResult.IsError)
                {
                    requestRecord.Client = clientResult.Client;
                }
            }

            foreach (var evaluator in _evaluators)
            {
                var directive = await ProcessPreEvaluatorAsync(evaluator, requestRecord);
                if (directive == RequestTrackerEvaluatorDirective.DenyRequest)
                {
                    return; // do not continue to the real IdentityServer4 middleware.
                }
            }

            scopedStorage.Storage["IdentityServerRequestRecord"] = requestRecord;
            //
            // The following invoke is letting the request continue on into the pipeline
            // 

            await _next(httpContext);
 
        }

        private void FixUpPath(HttpContext httpContext)
        {
            var query = from item in KnownEndpointPaths
                where httpContext.Request.Path.Value.StartsWith(item)
                select item;
            if (query.Any())
            {
                httpContext.Request.Path = $"{PathRootUrl}{httpContext.Request.Path}";
            }
        }

        private async Task<RequestTrackerEvaluatorDirective> ProcessPreEvaluatorAsync(
            IIdentityServerRequestTrackerEvaluator evaluator, 
            IdentityServerRequestRecord requestRecord)
        {
            string name = "";
            RequestTrackerEvaluatorDirective directive = RequestTrackerEvaluatorDirective.AllowRequest;
            try
            {
                name = evaluator.Name;
                var result = await evaluator.PreEvaluateAsync(requestRecord);
                directive = RequestTrackerEvaluatorDirective.AllowRequest;
                if (result != null)
                {
                    await result.ProcessAsync(requestRecord.HttpContext);
                    directive = result.Directive;
                }
               
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