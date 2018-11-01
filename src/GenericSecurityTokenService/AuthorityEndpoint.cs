using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GenericSecurityTokenService.Extensions;
using GenericSecurityTokenService.Functions;
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
        [FunctionName("authority")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "authority/{*all}")]
            HttpRequestMessage reqMessage,
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            var httpContextAccessor = FunctionStartup.EstablishHttpContextAccessor(context, reqMessage, req);
            FunctionStartup.EstablishContextAccessor(context);
          
            var factory = FunctionStartup.GetFactory(context);
            var functionHandler = factory.Create<IAuthorityFunction>();
        
            await functionHandler.InvokeAsync();

            return httpContextAccessor.HttpResponseMessage;
        }
    }
}
