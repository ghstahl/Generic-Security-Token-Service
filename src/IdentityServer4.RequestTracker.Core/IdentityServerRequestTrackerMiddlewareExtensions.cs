using System;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerRequestTracker
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IdentityServerRequestTrackerMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdentityServerRequestTrackerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdentityServerRequestTrackerMiddleware>();
        }
        public static IServiceCollection AddIdentityServerRequestTrackerMiddleware(this IServiceCollection services)
        {
            return services;
        }
    }
}
