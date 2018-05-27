using Microsoft.AspNetCore.Builder;

namespace P7.Core.Middleware
{
    public static class ProtectLocalOnlyExtensions
    {
        public static IApplicationBuilder UseProtectLocalOnly(this IApplicationBuilder builder, ProtectLocalOnlyOptions options)
        {
            return builder.UseMiddleware<ProtectLocalOnly>(options);
        }
    }
}