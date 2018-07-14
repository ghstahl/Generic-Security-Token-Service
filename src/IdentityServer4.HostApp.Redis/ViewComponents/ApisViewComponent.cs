using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using IdentityServer4Extras.Stores;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.ViewComponents
{
    public class ApisViewComponent : ViewComponent
    {
        private IResourceStore _resourceStore;

        public ApisViewComponent(IResourceStore resourceStore)
        {
            _resourceStore = resourceStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var resources = await _resourceStore.GetAllResourcesAsync();
            return View(resources.ApiResources.ToList());
        }
    }
}
