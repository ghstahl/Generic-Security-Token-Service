using AspNetCoreRateLimit.Core.Store;
using AspNetCoreRateLimit.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCoreRateLimit
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIpRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpRateLimitMiddleware>();
        }

        public static IApplicationBuilder UseClientRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientRateLimitMiddleware>();
        }

        public static void AddClientRateLimitServices(this IServiceCollection services)
        {
            services.AddSingleton<IClientRateLimitProcessor, ClientRateLimitProcessor>();
            services.AddSingleton<IClientRateLimitCounterStore, ClientRateLimitCounterStore>();
        }
        public static void AddHeaderClientRequestStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientRequestStore, HeaderClientRequestStore>();
        }
    }
}
