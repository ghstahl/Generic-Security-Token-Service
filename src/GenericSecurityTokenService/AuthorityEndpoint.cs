using System;
using System.Net.Http;
using System.Threading.Tasks;
using GenericSecurityTokenService.Extensions;
using GenericSecurityTokenService.Functions;
using GenericSecurityTokenService.Modules;
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
    public static class AuthorityEndpoint
    {
        private static IFunctionFactory _factory;

        private static IFunctionFactory GetFactory(ExecutionContext context)
        {
            return _factory ?? (_factory = new CoreFunctionFactory(new CoreAppModule(context.FunctionAppDirectory)));
        }

        private static void EstablishHttpContextAccessor(
            ExecutionContext context,
            HttpRequest req
        )
        {
            var factory = GetFactory(context);
            var httpAccessor = factory.ServiceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var response = new MyHttpResponse(req);
            var httpContext = new MyHttpContext(factory.ServiceProvider, req, response);
            httpContext.SetIdentityServerBasePath("/api/authority");
            httpAccessor.HttpContext = httpContext;
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
        [FunctionName("authority")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "authority/{*all}")]
            HttpRequestMessage reqMessage,
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            EstablishHttpContextAccessor(context, req);
            EstablishContextAccessor(context);
          
            var factory = GetFactory(context);
            var functionHandler = factory.Create<IAuthorityFunction>();
            var result = await functionHandler.InvokeAsync();
            return result;
        }
    }
}
