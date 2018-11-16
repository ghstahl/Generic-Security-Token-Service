using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Core.Store;
using IdentityServer4.Models;
using IdentityServer4Extras;
using IdentityServer4Extras.Stores;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.ViewComponents
{
    public class ClientViewComponentModel
    {
        public ClientExtra ClientExtra { get; set; }
        public List<RateLimitCounterRecord> RateLimitCounterRecords { get; set; }
    }
    public class ClientViewComponent : ViewComponent
    {
        private IClientStoreExtra _clientStore;
        private IClientRateLimitCounterStore _clientRateLimitCounterStore;

        public ClientViewComponent(IClientStoreExtra clientStore, IClientRateLimitCounterStore clientRateLimitCounterStore)
        {
            _clientStore = clientStore;
            _clientRateLimitCounterStore = clientRateLimitCounterStore;
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
            var rateLimitRecords = await _clientRateLimitCounterStore.GetRateLimitCounterRecordsAsync(
                new ClientRequestIdentity()
                {
                    ClientId = clientId
                });
            var model = new ClientViewComponentModel
            {
                ClientExtra = client as ClientExtra,
                RateLimitCounterRecords = rateLimitRecords.ToList()
            };
            return View(model);
        }
    }
}