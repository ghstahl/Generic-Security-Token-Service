using System.Threading.Tasks;
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
            var client = await _clientStore.FindClientByIdAsync(id);
            return View(client as ClientExtra);
        }
    }
}