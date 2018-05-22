using System.Threading.Tasks;
using IdentityServer4Extras.Stores;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.ViewComponents
{
    public class ClientsViewComponent : ViewComponent
    {
        private IClientStoreExtra _clientStore;

        public ClientsViewComponent(IClientStoreExtra clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var clients = await _clientStore.GetAllClientIdsAsync();
            return View(clients);
        }
    }
}
