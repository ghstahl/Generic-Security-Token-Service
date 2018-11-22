using IdentityServerRequestTracker.Middleware;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerRequestTracker.Extensions
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIdentityServerRequestTrackerMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<PreIdentityServerMiddleware>();
            builder.UseMiddleware<IdentityServerRequestTrackerMiddleware>();
            builder.UseMiddleware<PostIdentityServerMiddleware>();
            return builder;
        }
       
        public static IServiceCollection AddIdentityServerRequestTrackerMiddleware(this IServiceCollection services)
        {
            services.AddTransient<IAllowRequestTrackerResult,AllowRequestTrackerResult>();
            services.AddScoped<IScopedStorage, ScopedStorage>();
            return services;
        }
    }
}
