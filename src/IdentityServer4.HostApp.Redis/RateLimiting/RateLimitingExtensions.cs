using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.HostApp.RateLimiting
{
    public static class RateLimitingExtensions
    {
        public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // needed to load configuration from appsettings.json
            services.AddOptions();

            //load general configuration from appsettings.json
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));

            //load client rules from appsettings.json
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));


            // inject counter and rules distributed cache stores
            services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();


            services.AddSingleton<IClientRateLimiterHandler, ClientRateLimiterHandler>();

        }
    }
}
