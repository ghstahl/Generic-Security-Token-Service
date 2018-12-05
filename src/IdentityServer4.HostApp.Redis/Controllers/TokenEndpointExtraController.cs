using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using IdentityServer4.Hosting;
using IdentityServer4Extras.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace IdentityServer4.HostApp.Controllers
{
    [Produces("application/json")]
    [Route("api/TokenTokenEndpointExtra")]
    public class TokenEndpointExtraController : Controller
    {
        private ITokenEndpointHandlerExtra _tokenEndpointHandlerExtra;

        public TokenEndpointExtraController(ITokenEndpointHandlerExtra tokenEndpointHandlerExtra)
        {
            _tokenEndpointHandlerExtra = tokenEndpointHandlerExtra;
        }

        // GET: api/Default
        [HttpGet]
        public async Task<IEndpointResult> GetAsync()
        {
            var arbitraryClaims = new Dictionary<string, List<string>>
            {
                {"some_guid", new List<string>() {Guid.NewGuid().ToString()}},
                {"in", new List<string>() {"flames"}}
            };
          
            var jsonArbitraryClaims = JsonConvert.SerializeObject(arbitraryClaims);

            IFormCollection formCollection = new FormCollection(new Dictionary<string, StringValues>()
            {
                {"grant_type", "arbitrary_resource_owner"},
                {"client_id", "arbitrary-resource-owner-client"},
                {"client_secret", "secret"},
                {"scope", "offline_access nitro metal"},
                {"arbitrary_claims", jsonArbitraryClaims},
                {"subject", "Ratt"},
                {"access_token_lifetime", "3600"},
            });

            return await _tokenEndpointHandlerExtra.ProcessAsync(formCollection);
        }
    }
}