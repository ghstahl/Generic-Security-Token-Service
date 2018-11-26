using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServerRequestTracker.Middleware
{
    public class PostIdentityServerMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<PostIdentityServerMiddleware> _logger;
        private IEnumerable<IIdentityServerRequestTrackerEvaluator> _evaluators;

        public PostIdentityServerMiddleware(
            RequestDelegate next,
            IEnumerable<IIdentityServerRequestTrackerEvaluator> evaluators, 
            ILogger<PostIdentityServerMiddleware> logger)
        {
            _next = next;
            _evaluators = evaluators;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, IScopedStorage scopedStorage)
        {
            var handled = (bool)scopedStorage.Storage["IdentityServer:Handled"];
            if (handled)
            {
                // this is where we get a chance to write those response headers on what we believe is the way out.
                // as we classify that when the IdentityServer middleware finishes it on the way out.
                var requestRecord = (IdentityServerRequestRecord)scopedStorage.Storage["IdentityServerRequestRecord"];
                var result = (IEndpointResult)scopedStorage.Storage["IEndpointResult"];
                var tokenResult = result as ITokenEndpointResult;
                var error = tokenResult == null;

                foreach (var evaluator in _evaluators)
                {
                    var directive = await ProcessPostEvaluatorAsync(evaluator, requestRecord, error);
                    if (directive == RequestTrackerEvaluatorDirective.DenyRequest)
                    {
                        return; // do not continue to the real IdentityServer4 middleware.
                    }
                }
                
                await result.ExecuteAsync(httpContext);

                return;
            }
            await _next(httpContext);
        }
        private async Task<RequestTrackerEvaluatorDirective> ProcessPostEvaluatorAsync(
            IIdentityServerRequestTrackerEvaluator evaluator,
            IdentityServerRequestRecord requestRecord,
            bool error)
        {
            string name = "";
            RequestTrackerEvaluatorDirective directive = RequestTrackerEvaluatorDirective.AllowRequest;
            try
            {
                name = evaluator.Name;
                var result = await evaluator.PostEvaluateAsync(requestRecord, error);
                directive = RequestTrackerEvaluatorDirective.AllowRequest;
                if (result != null)
                {
                    await result.ProcessAsync(requestRecord.HttpContext);
                    directive = result.Directive;
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"EvaluatorProcessed Exception,message={e.Message} ");
            }
            _logger.LogInformation($"EvaluatorProcessed,name={name},directive={directive}");
            return directive;
        }
    }
}