using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4Extras;
using IdentityServer4Extras.Stores;
using IdentityServerRequestTracker.RateLimit.Options;
using IdentityServerRequestTracker.RateLimit.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.ViewComponents
{
    public class RateLimitRuleRecord
    {
        public RateLimitCounter? RateLimitCounter { get; set; }
        public RateLimitRule RateLimitRule { get; set; }

    }
    public class ClientViewComponentModel
    {
        public ClientExtra ClientExtra { get; set; }
        public List<RateLimitRuleRecord> RateLimitRuleRecords { get; set; }
    }
    public class ClientViewComponent : ViewComponent
    {
        private IClientStoreExtra _clientStore;
        private IClientRateLimitProcessor _processor;

        public ClientViewComponent(IClientStoreExtra clientStore, IClientRateLimitProcessor processor)
        {
            _clientStore = clientStore;
            _processor = processor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
           
            string clientId = id;
            if (string.IsNullOrWhiteSpace(clientId))
            {
                var clients = await _clientStore.GetAllClientIdsAsync();
                clientId = clients.FirstOrDefault();
            }
            var client = await _clientStore.FindClientByIdAsync(clientId);
            var clientRequestIdentity =  new ClientRequestIdentity()
            {
                ClientId = clientId
            };

            var rateLimitClientsRule = _processor.GetRateLimitClientsRule(clientRequestIdentity);
            List<RateLimitRuleRecord> rateLimitRuleRecords = new List<RateLimitRuleRecord>();
            if (rateLimitClientsRule != null)
            {
                foreach (var rateLimitRule in rateLimitClientsRule.Settings.RateLimitRules)
                {
                    var rateLimitCounter = _processor.GetStoredRateLimitCounter(clientRequestIdentity, rateLimitRule);
                    rateLimitRuleRecords.Add(new RateLimitRuleRecord()
                    {
                        RateLimitCounter = rateLimitCounter,
                        RateLimitRule = rateLimitRule
                    });
                }
            }

            var model = new ClientViewComponentModel
            {
                ClientExtra = client as ClientExtra,
                RateLimitRuleRecords = rateLimitRuleRecords 
            };
            return View(model);
        }
    }
}