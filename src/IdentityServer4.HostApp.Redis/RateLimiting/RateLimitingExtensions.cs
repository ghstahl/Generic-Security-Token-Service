using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using ClientIdRateLimitStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer4.HostApp.RateLimiting
{
    public class MyClientRateLimitOptions : ClientRateLimitOptions
    {

    }
    public class MyClientRateLimitOptionsAccessor : IOptions<ClientRateLimitOptions>
    {
        private MyClientRateLimitOptions _optionsValue;

        public MyClientRateLimitOptionsAccessor(IOptions<MyClientRateLimitOptions> options)
        {
            _optionsValue = options.Value;
        }
        public ClientRateLimitOptions Value => _optionsValue;
    }
    public static class RateLimitingExtensions
    {
        public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
          

            //load general configuration from appsettings.json
            services.Configure<MyClientRateLimitOptions>(configuration.GetSection("IdentityServerClientRateLimiting"));
            services.AddSingleton<IOptions<ClientRateLimitOptions>, MyClientRateLimitOptionsAccessor>();

            //load client rules from appsettings.json
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));


            // inject counter and rules distributed cache stores
            services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            services.AddIdentityServerClientRequestStore();
            services.AddClientRateLimitServices();
        }
    }
}
