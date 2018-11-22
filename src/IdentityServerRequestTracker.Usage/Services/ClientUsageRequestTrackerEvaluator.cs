using System;
using System.Net;
using System.Threading.Tasks;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerRequestTracker.Usage.Services
{
    internal class ClientUsageRequestTrackerEvaluator : IIdentityServerRequestTrackerEvaluator
    {
        private IServiceProvider _serviceProvider;
       
        private ILogger<ClientUsageRequestTrackerEvaluator> _logger;
        private IClientUsageStore _clientUsageStore;

        public ClientUsageRequestTrackerEvaluator(
            IServiceProvider serviceProvider,
            IClientUsageStore clientUsageStore,
            ILogger<ClientUsageRequestTrackerEvaluator> logger
        )
        {
            _serviceProvider = serviceProvider;
            _clientUsageStore = clientUsageStore;
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
            var context = requestRecord.HttpContext;
            var grantType = "";
            if (HttpMethods.IsPost(context.Request.Method))
            {
                if (context.Request.HasFormContentType)
                {
                    var values = context.Request.Form.AsNameValueCollection();
                    grantType = values.Get("grant_type");
                     
                }

            }

            var record = new ClientUsageRecord()
            {
                ClientId = clientId,
                DateTime = DateTime.UtcNow,
                EndPointKey = endpointKey,
                GrantType = grantType
            };
            await _clientUsageStore.TrackAsync(record);
            return null;
        }
    }
}
