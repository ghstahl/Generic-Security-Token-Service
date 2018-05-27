using Microsoft.AspNetCore.Builder;

namespace P7.Core.Middleware
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ProtectPathExtensions
    {
        public static IApplicationBuilder UseProtectPath(this IApplicationBuilder builder, ProtectPathOptions options)
        {
            return builder.UseMiddleware<ProtectPath>(options);
        }
    }
}
