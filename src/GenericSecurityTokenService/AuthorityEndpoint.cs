using System;
using System.IO;
using System.Threading.Tasks;
using GenericSecurityTokenService.Functions;
using GenericSecurityTokenService.Functions.FunctionOptions;
using GenericSecurityTokenService.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GenericSecurityTokenService
{
    
    public static class AuthorityEndpoint
    {
        private static IFunctionFactory _factory;

        private static IFunctionFactory GetFactory(ExecutionContext context)
        {
            if (_factory == null)
            {
                _factory = new CoreFunctionFactory(new CoreAppModule(context.FunctionAppDirectory));
            }

            return _factory;
        }
      

        [FunctionName("Authority")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {

            var options = GetOptions(req);
            var factory = GetFactory(context);
            var result = await factory.Create<IHelloFunction>(log).InvokeAsync(req, options)
                .ConfigureAwait(false);

            return result;
            /*
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult) new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
                */
        }

        private static DefaultHttpTriggerOptions GetOptions(HttpRequest req)
        {
            return new DefaultHttpTriggerOptions(req);
        }
    }
}
