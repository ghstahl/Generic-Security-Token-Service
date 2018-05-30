using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4Extras.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using P7.Core.Scheduler.Scheduling;

namespace P7IdentityServer4.Cron
{
    /// /////////////////////////////////////////
    //  "TokenEndpointHealthTask": {
    //        "clientId": "arbitrary-resource-owner-client",
    //        "clientSecret": "secret",
    //        "scheduleCron": "*/5 * * * *" 
    //        }
    /// /////////////////////////////////////////

    public class TokenEndpointHealthTask : IScheduledTask
    {
        private IConfiguration _configuration;
        private ILogger _logger;
        private const string Every5Minutes = "*/5 * * * *"; //https://crontab.guru/every-5-minutes
        private IEndpointHandlerExtra _endpointHandlerExtra;
        private IServiceProvider _serviceProvider;

        public TokenEndpointHealthTask(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<KeyVaultCache> logger,
            IKeyVaultCache keyVaultCache)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            Schedule = _configuration["TokenEndpointHealthTask:scheduleCron"];
            if (string.IsNullOrWhiteSpace(Schedule))
            {
                Schedule = Every5Minutes;
            }
        }

        public string Schedule { get; }


        public async Task Invoke(CancellationToken cancellationToken)
        {
            var key = "TokenEndpointHealthTask";
            var keyKeyVaultRefresh = "keyvault-data-refresh";
           

            IEndpointHandlerExtra endpoint =
                _serviceProvider.GetService(typeof(IEndpointHandlerExtra)) as IEndpointHandlerExtra;
             

            bool success = false;
            try
            {
                var arbitraryClaims = new Dictionary<string, List<string>>
                {
                    {"some_guid", new List<string>() {Guid.NewGuid().ToString()}},
                    {"in", new List<string>() {"flames"}}
                };
                var jsonArbitraryClaims = JsonConvert.SerializeObject(arbitraryClaims);

                IFormCollection formCollection = new FormCollection(new Dictionary<string, StringValues>()
                {
                    {"grant_type", "arbitrary_resource_owner"},
                    {"client_id", _configuration["TokenEndpointHealthTask:clientId"]},
                    {"client_secret", _configuration["TokenEndpointHealthTask:clientSecret"]},
                    {"scope", "offline_access nitro metal"},
                    {"arbitrary_claims", jsonArbitraryClaims},
                    {"subject", "flatratt-e1da-4243-84d0-42233b145382"},
                    {"access_token_lifetime", "360"},
                });
                var result = await endpoint.ProcessRawAsync(formCollection);
                success = result.TokenResult != null;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cron job to pull keyvault data failure!");
            }
             
        }
    }
}