
using Microsoft.AspNetCore.Http;

namespace P7.Core.Middleware
{
    public interface IMiddlewarePlugin
    {
        bool Invoke(HttpContext httpContext);
    }
}