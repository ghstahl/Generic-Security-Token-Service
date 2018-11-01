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
        private static IFunctionFactory _factory;

        private static IFunctionFactory GetFactory(ExecutionContext context)
        {
            return _factory ?? (_factory = new CoreFunctionFactory(new CoreAppModule(context.FunctionAppDirectory)));
        }

        private static IHttpContextAccessor EstablishHttpContextAccessor(
            ExecutionContext context,
            HttpRequestMessage reqMessage,
            HttpRequest req
        )
        {
            var factory = GetFactory(context);
            var httpContextAccessor = factory.ServiceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var response = new MyHttpResponse(req);
            var httpContext = new MyHttpContext(factory.ServiceProvider, req, response);
            httpContextAccessor.HttpContext = httpContext;
            return httpContextAccessor;
        }

        private static void EstablishContextAccessor(
            ExecutionContext context)
        {
            var factory = GetFactory(context);
            var myContextAccessor =
                factory.ServiceProvider.GetService(typeof(IMyContextAccessor)) as IMyContextAccessor;
            myContextAccessor.MyContext = new MyContext();
            myContextAccessor.MyContext.Dictionary["tt"] = Guid.NewGuid().ToString();
        }
        [FunctionName("identity")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "identity")]
            HttpRequestMessage reqMessage,
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            var httpContextAccessor = EstablishHttpContextAccessor(context,reqMessage,req);
            EstablishContextAccessor(context);

            var factory = GetFactory(context);
            var tokenValidator = factory.ServiceProvider.GetService(typeof(ITokenValidator)) as ITokenValidator;
            httpContextAccessor.HttpContext.User = await tokenValidator.ValidateTokenAsync(reqMessage.Headers.Authorization); ;

            if (httpContextAccessor.HttpContext.User == null)
            {
                return reqMessage.CreateResponse(HttpStatusCode.Unauthorized);
            }
            // Authentication boilerplate code end

            return reqMessage.CreateResponse(HttpStatusCode.OK, "Hello " + httpContextAccessor.HttpContext.User.Identity.Name);
        }
    }
}
