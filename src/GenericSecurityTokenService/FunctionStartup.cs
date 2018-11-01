using System;
using System.Net.Http;
using GenericSecurityTokenService.Functions;
using GenericSecurityTokenService.Modules;
using GenericSecurityTokenService.Security;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;

namespace GenericSecurityTokenService
{
    public static class FunctionStartup
    {
        private static IFunctionFactory _factory;

        public static IFunctionFactory GetFactory(ExecutionContext context)
        {
            return _factory ?? (_factory = new CoreFunctionFactory(new CoreAppModule(context.FunctionAppDirectory)));
        }

        public static IFunctionHttpContextAccessor EstablishHttpContextAccessor(
            ExecutionContext context,
            HttpRequestMessage reqMessage,
            HttpRequest req
        )
        {
            var factory = GetFactory(context);
            var httpAccessor = factory.ServiceProvider.GetService(typeof(IFunctionHttpContextAccessor)) as IFunctionHttpContextAccessor;
            var response = new MyHttpResponse(req);
            var httpContext = new MyHttpContext(factory.ServiceProvider, req, response);
            httpContext.SetIdentityServerBasePath("/api/authority");
            httpAccessor.HttpContext = httpContext;
            httpAccessor.HttpRequestMessage = reqMessage;
            httpAccessor.HttpResponseMessage = reqMessage.CreateResponse();
            return httpAccessor;
        }

        public static async Task EstablishIdentityAsync(
            ExecutionContext context, IFunctionHttpContextAccessor httpContextAccessor)
        {
     
            var factory = GetFactory(context);
            var tokenValidator = factory.ServiceProvider.GetService(typeof(ITokenValidator)) as ITokenValidator;

            httpContextAccessor.HttpContext.User =
                await tokenValidator.ValidateTokenAsync(httpContextAccessor.HttpRequestMessage.Headers.Authorization);
        }
        public static void EstablishContextAccessor(
            ExecutionContext context)
        {
            var factory = GetFactory(context);
            var myContextAccessor =
                factory.ServiceProvider.GetService(typeof(IMyContextAccessor)) as IMyContextAccessor;
            myContextAccessor.MyContext = new MyContext();
            myContextAccessor.MyContext.Dictionary["tt"] = Guid.NewGuid().ToString();
        }
    }
}