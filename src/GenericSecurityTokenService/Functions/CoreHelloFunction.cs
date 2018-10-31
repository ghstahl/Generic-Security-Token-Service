using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GenericSecurityTokenService.Functions.FunctionOptions;
using GenericSecurityTokenService.Modules;
using IdentityServer4;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GenericSecurityTokenService.Functions
{
    /// <summary>
    /// This represents the function entity for hello.
    /// </summary>
    public class CoreHelloFunction : IHelloFunction
    {
        private IEndpointRouter _endpointRouter;
        private IServiceProvider _serviceProvider;
        private IMyContextAccessor _myContextAccessor;
        private IHttpContextAccessor _httpContextAccessor;

        public CoreHelloFunction(
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient,
            IMyContextAccessor myContextAccessor,
            IServiceProvider serviceProvider,
            IEndpointRouter endpointRouter)
        {
            _httpContextAccessor = httpContextAccessor;
            _myContextAccessor = myContextAccessor;
            _serviceProvider = serviceProvider;
            _endpointRouter = endpointRouter;
        }
        public ILogger Log { get; set; }
        public async Task<ActionResult> InvokeAsync()
        {

            var endpointHandler = _endpointRouter.Find(_httpContextAccessor.HttpContext);
            if (endpointHandler != null)
            {
                var endpointResult = await endpointHandler.ProcessAsync(_httpContextAccessor.HttpContext);
                var endpointResult2 = endpointResult as IEndpointResult2;
                var json = "";
                //await result.BuildResponseAsync(httpContext);
                await endpointResult.ExecuteAsync(_httpContextAccessor.HttpContext);
                _httpContextAccessor.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                StreamReader rdr = new StreamReader(_httpContextAccessor.HttpContext.Response.Body, Encoding.UTF8);
                json = rdr.ReadToEnd();
                var result = new JsonResult(endpointResult2.Value);
                return result;
            }
            Log.LogInformation("C# HTTP trigger function processed a request.");
          
            var res = new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            return res;
        }
    }
}