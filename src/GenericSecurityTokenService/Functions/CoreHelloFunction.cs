using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GenericSecurityTokenService.Functions.FunctionOptions;
using Microsoft.AspNetCore.Mvc;
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
       
        private readonly HttpClient _httpClient;

        public CoreHelloFunction(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        public ILogger Log { get; set; }
        public async Task<ActionResult> InvokeAsync<TInput>(TInput input, FunctionOptionsBase options)
        {
            Log.LogInformation("C# HTTP trigger function processed a request.");
            var option = options as DefaultHttpTriggerOptions;

            string name = option.HttpRequest.Query["name"];

            string requestBody = await new StreamReader(option.HttpRequest.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var res = name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            return res;
        }
    }
}