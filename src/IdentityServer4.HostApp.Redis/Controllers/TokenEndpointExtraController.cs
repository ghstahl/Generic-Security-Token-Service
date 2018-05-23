using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Hosting;
using IdentityServer4Extras.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.HostApp.Redis.Controllers
{
    [Produces("application/json")]
    [Route("api/TokenEndpointExtra")]
    public class TokenEndpointExtraController : Controller
    {
        private IEndpointHandlerExtra _endpointHandlerExtra;

        public TokenEndpointExtraController(IEndpointHandlerExtra endpointHandlerExtra)
        {
            _endpointHandlerExtra = endpointHandlerExtra;
        }
           // POST: api/Default
           [HttpPost]
        public async Task<IEndpointResult> PostAsync()
           {
               return await _endpointHandlerExtra.ProcessAsync(HttpContext);
           }
    }
}