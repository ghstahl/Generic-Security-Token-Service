using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using GenericSecurityTokenService.Extensions;
using GenericSecurityTokenService.Functions;
using GenericSecurityTokenService.Modules;
using GenericSecurityTokenService.Security;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace GenericSecurityTokenService
{
    public static class IdentityEndpoint
    {
        [FunctionName("identity")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "identity")]
            HttpRequestMessage reqMessage,
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            var httpContextAccessor = FunctionStartup.EstablishHttpContextAccessor(context, reqMessage, req);
            FunctionStartup.EstablishContextAccessor(context);

            var factory = FunctionStartup.GetFactory(context);
            var functionHandler = factory.Create<IIdentityFunction>();
            await functionHandler.InvokeAsync();

            return httpContextAccessor.HttpResponseMessage;
            /*

            var tokenValidator = factory.ServiceProvider.GetService(typeof(ITokenValidator)) as ITokenValidator;
            httpContextAccessor.HttpContext.User = await tokenValidator.ValidateTokenAsync(reqMessage.Headers.Authorization); ;

            if (httpContextAccessor.HttpContext.User == null)
            {
                return reqMessage.CreateResponse(HttpStatusCode.Unauthorized);
            }
            // Authentication boilerplate code end

            return reqMessage.CreateResponse(HttpStatusCode.OK, "Hello " + httpContextAccessor.HttpContext.User.Identity.Name);
            */
        }
    }
}
