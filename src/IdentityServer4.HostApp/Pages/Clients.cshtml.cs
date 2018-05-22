using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer4.HostApp.Pages
{
    public class ClientsModel : PageModel
    {
        public string ClientId { get; private set; }
        public async Task OnGetAsync(string id)
        {
            ClientId = id;
        }
    }
}