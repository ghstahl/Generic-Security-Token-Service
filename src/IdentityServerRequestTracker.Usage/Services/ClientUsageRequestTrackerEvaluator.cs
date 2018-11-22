using System;
using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerRequestTracker.Usage.Services
{
    internal class ClientUsageRequestTrackerEvaluator : IIdentityServerRequestTrackerEvaluator
    {
        private IServiceProvider _serviceProvider;
       
        private ILogger<ClientUsageRequestTrackerEvaluator> _logger;

        public ClientUsageRequestTrackerEvaluator(
            IServiceProvider serviceProvider, 
            ILogger<ClientUsageRequestTrackerEvaluator> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            Name = "client_usage";
        }

        public string Name { get; set; }
        public async Task<IRequestTrackerResult> PreEvaluateAsync(IdentityServerRequestRecord requestRecord)
        {
            // we only care about successful requests
            return null;
        }

        public async Task<IRequestTrackerResult> PostEvaluateAsync(IdentityServerRequestRecord requestRecord, bool error)
        {
            if (requestRecord.Client == null)
            {
                // not for us
                return null;
            }
            if (error)
            {
                return null;
            }
            
            var clientId = requestRecord.Client.ClientId;
            var endpointKey = requestRecord.EndpointKey;

            return null;
        }
    }
}
