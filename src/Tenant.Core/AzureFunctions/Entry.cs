using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Tenant.Core.Shims;

namespace Tenant.Core.AzureFunctions
{
    public static class Entry<THost, TStartup> 
        where THost : class
        where TStartup : class
    {
        public static async Task<HttpResponseMessage> Run(
            Microsoft.Azure.WebJobs.ExecutionContext context,
            HttpRequest request,
            ILogger log) 
          
        {
            try
            {
                var configuration = HostConfiguration<THost>.GetConfiguration(context.FunctionAppDirectory);
                var serverRecords = TenantApps<TStartup>.GetServerRecords(context.FunctionAppDirectory, configuration, log);

                var path = new PathString(request.Path.Value.ToLower());
                log.LogInformation($"C# HTTP trigger:{request.Method} {path}.");

                var query = from item in serverRecords
                    where path.StartsWithSegments(item.PathStringBaseUrl)
                    select item;
                var serverRecord = query.FirstOrDefault();
                HttpResponseMessage response = null;
                if (serverRecord == null)
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return response;
                }


                var httpRequestMessageFeature = new TenantHttpRequestMessageFeature(request);
                var httpRequestMessage = httpRequestMessageFeature.HttpRequestMessage;

                HttpClient client = serverRecord.TestServer.CreateClient();
                client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}");

                // trim off the front router hints
                path = path.Value.Substring(serverRecord.PathStringBaseUrl.Value.Length);
                var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host)
                {
                    Path = path,
                    Query = request.QueryString.Value
                };
                if (request.Host.Port != null)
                {
                    uriBuilder.Port = (int)request.Host.Port;
                }

                httpRequestMessage.RequestUri = uriBuilder.Uri;
                httpRequestMessage.Headers.Remove("Host");
                var responseMessage = await client.SendAsync(httpRequestMessage);
                return responseMessage;
            }
            catch (Exception e)
            {
                log.LogError(e, $"MainEntry Exception:{e.Message}");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
