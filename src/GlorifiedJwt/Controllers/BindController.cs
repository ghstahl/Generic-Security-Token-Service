using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServer4Extras.Endpoints;
using IdentityServer4Extras.Stores;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using P7.Core.Cache;

namespace GlorifiedJwt.Controllers
{
    class CustomPayload
    {
        public string A { get; set; }
        public string B { get; set; }
        public List<string> CList => new List<string>() {"a", "b", "c"};
    }
    [Route("api/[controller]")]
    [ApiController]
    public class BindController : ControllerBase
    {
        private ILogger<BindController> _logger;
        private ITokenEndpointHandlerExtra _tokenEndpointHandlerExtra;

        public BindController(
            ITokenEndpointHandlerExtra tokenEndpointHandlerExtra,
            ILogger<BindController> logger)
        {
            _tokenEndpointHandlerExtra = tokenEndpointHandlerExtra;
            _logger = logger;
        }


        // GET api/values
        [HttpPost]
        public async Task<IEndpointResult> PostAsync()
        {
            _logger.LogInformation("Summary Executing...");
            var extensionGrantRequest = new ExtensionGrantRequest()
            {
                ClientId = "arbitrary-resource-owner-client",
                GrantType = "arbitrary_resource_owner",
                Scopes = new List<string>()
                {
                    "offline_access", "metal", "nitro", "In", "Flames"
                },
                Subject = "PorkyPig",
                ArbitraryClaims = new Dictionary<string, List<string>>()
                {
                    {"top",new List<string>(){"dog"}},
                    {"role",new List<string>(){"application","limited"}},
                    {"query",new List<string>(){ "dashboard", "licensing"}},
                    {"seatId",new List<string>(){"2368d213-d06c-4c2a-a099-11c34adc357"}},
                    {"piid",new List<string>(){"2368d213-1111-4c2a-a099-11c34adc3579"}}
                },
                AccessTokenLifetime = "3600",
                ArbitraryAmrs = new List<string>()
                {
                    "agent:username:agent0@supporttech.com",
                    "agent:challenge:fullSSN",
                    "agent:challenge:homeZip"
                },
                ArbitraryAudiences = new List<string>() { "cat","dog" },
                CustomPayload = new CustomPayload()
            };

          
            var result = await _tokenEndpointHandlerExtra.ProcessAsync(extensionGrantRequest);
            return result;
        }
    }
}