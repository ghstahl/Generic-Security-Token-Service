
using Microsoft.AspNetCore.Http;

namespace P7.Core.Middleware
{
    public abstract class MiddlewarePlugin : IMiddlewarePlugin
    {
        public abstract bool Invoke(HttpContext httpContext);
    }
}