using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace P7.Core.Middleware
{
    public class HostNameRewriteMiddleware
    {
        private readonly RequestDelegate _next;

        public HostNameRewriteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("x-hostname-rewrite"))
            {
                var hostName = context.Request.Headers["x-hostname-rewrite"];
                int? port = context.Request.Host.Port;
                var hostString = (port == null) ? new HostString(hostName) : new HostString(hostName, (int)port);
                context.Request.Host = hostString;
            }
            await _next(context);
        }
    }
}