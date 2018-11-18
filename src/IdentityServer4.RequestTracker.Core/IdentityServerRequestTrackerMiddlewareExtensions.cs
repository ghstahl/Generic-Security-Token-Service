using System;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4RequestTracker
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IdentityServerRequestTrackerMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdentityServer4RequestTrackerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdentityServerRequestTrackerMiddleware>();
        }
        public static IServiceCollection AddIdentityServer4RequestTrackerMiddleware(this IServiceCollection services)
        {
            return services;
        }
    }
}
