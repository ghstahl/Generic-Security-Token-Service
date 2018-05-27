using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using P7.Core.Deployment;

namespace P7.Core.Middleware
{
    public class HostRewriteMiddleware
    {
        private readonly RequestDelegate _next;
        private IOptions<DeploymentOptions> _deploymentOptions;

        public HostRewriteMiddleware(RequestDelegate next,
            IOptions<DeploymentOptions> deploymentOptions)
        {
            _next = next;
            _deploymentOptions = deploymentOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(_deploymentOptions.Value.Host))
                {
                    int? port = context.Request.Host.Port;
                    var host = _deploymentOptions.Value.Host;
                    var hostString = (port == null) ? new HostString(host) : new HostString(host, (int) port);
                    context.Request.Host = hostString;

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            await _next(context);
        }
    }
}