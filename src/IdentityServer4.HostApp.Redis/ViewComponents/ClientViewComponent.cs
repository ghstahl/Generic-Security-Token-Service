using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4Extras;
using IdentityServer4Extras.Stores;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.ViewComponents
{
    public class ClientViewComponent : ViewComponent
    {
        private IClientStoreExtra _clientStore;

        public ClientViewComponent(IClientStoreExtra clientStore)
        {
            _clientStore = clientStore;
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
            return View(client as ClientExtra);
        }
    }
}