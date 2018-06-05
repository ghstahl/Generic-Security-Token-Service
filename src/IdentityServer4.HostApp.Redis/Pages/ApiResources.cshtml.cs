using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer4.HostApp.Redis.Pages
{
    public class ApiResourcesModel : PageModel
    {
        public string ClientId { get; private set; }
        public async Task OnGetAsync(string apiResourceId)
        {
            ClientId = apiResourceId;
        }
    }
}