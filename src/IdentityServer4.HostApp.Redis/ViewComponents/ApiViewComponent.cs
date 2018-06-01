using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4Extras;
using IdentityServer4Extras.Stores;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.Redis.ViewComponents
{
    public class ApiViewComponentModel
    {
        public ApiResource ApiResource { get; set; }
         
    }
    public class ApiViewComponent : ViewComponent
    {
        private IResourceStore _resourceStore;

        public ApiViewComponent(IResourceStore resourceStore)
        {
            _resourceStore = resourceStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var model = new ApiViewComponentModel { };
            if (string.IsNullOrWhiteSpace(id))
            {
                var resources = await _resourceStore.GetAllResourcesAsync();
                var resource = resources.ApiResources.FirstOrDefault();
                if (resource != null)
                {
                    model.ApiResource = resource;
                }
            }
            else
            {
                var resource = await _resourceStore.FindApiResourceAsync(id);
                model.ApiResource = resource;
            }

            return View(model);
        }
    }
}