using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer4.HostApp.Pages
{
    public class ClientsModel : PageModel
    {
        public string ClientId { get; private set; }
        public async Task OnGetAsync(string clientId)
        {
            ClientId = clientId;
        }
    }
}