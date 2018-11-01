using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GenericSecurityTokenService.Modules;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Hosting;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GenericSecurityTokenService.Functions
{
    /// <summary>
    /// This represents the function entity for CoreAuthorityFunction.
    /// </summary>
    public class CoreAuthorityFunction : IAuthorityFunction
    {
        private IEndpointRouter _endpointRouter;
        private IServiceProvider _serviceProvider;
        private IMyContextAccessor _myContextAccessor;
        private IHttpContextAccessor _httpContextAccessor;
        private IEventService _events;
        private ILogger _logger;

        public CoreAuthorityFunction(
            IHttpContextAccessor httpContextAccessor,
            IMyContextAccessor myContextAccessor,
            IServiceProvider serviceProvider,
            IEventService events,
            IEndpointRouter endpointRouter,
            ILogger<CoreAuthorityFunction> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _myContextAccessor = myContextAccessor;
            _serviceProvider = serviceProvider;
            _events = events;
            _endpointRouter = endpointRouter;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpResponseMessage httpResponseMessage)
        {
            ActionResult result = null;
            bool executed = false;
            try
            {
                _logger.LogInformation("C# HTTP trigger Authority function processed a request.");
                var context = _httpContextAccessor.HttpContext;
                var endpointHandler = _endpointRouter.Find(_httpContextAccessor.HttpContext);
                if (endpointHandler != null)
                {
                    _logger.LogInformation("Invoking IdentityServer endpoint: {endpointType} for {url}",
                        endpointHandler.GetType().FullName, context.Request.Path.ToString());

                    _logger.LogInformation(
                        $"Found IdentityServer endpoint {_httpContextAccessor.HttpContext.Request.Path}.");
                    var endpointResult = await endpointHandler.ProcessAsync(_httpContextAccessor.HttpContext);
                    var endpointResult2 = endpointResult as IEndpointResult2;

                    if (endpointResult2 != null)
                    {
                        /*
                        * More effective if we get the object value directly from the endpointResult.
                        * Need to ask IdentityServer to expose that inner value.
                        * I did it temporarily by modifying the IdentityServer4 code by added the IEndpointResult2
                        * interface to the endpoint results.
                        */
                        _logger.LogTrace("Invoking result: {type}", endpointResult2.GetType().FullName);
                        await endpointResult2.ExecuteAsync(httpResponseMessage);
                        executed = true;

                    }

                    /*
                        var json = "";

                        await endpointResult.ExecuteAsync(_httpContextAccessor.HttpContext);

                        _httpContextAccessor.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                        StreamReader rdr = new StreamReader(_httpContextAccessor.HttpContext.Response.Body, Encoding.UTF8);
                        json = rdr.ReadToEnd();
                    */
                }
            }
            catch (Exception ex)
            {
                await _events.RaiseAsync(new UnhandledExceptionEvent(ex));
                _logger.LogCritical(ex, "Unhandled exception: {exception}", ex.Message);
                throw;
            }

            if (!executed)
            {
                httpResponseMessage.StatusCode = HttpStatusCode.NotFound;
            }
        }
    }
}