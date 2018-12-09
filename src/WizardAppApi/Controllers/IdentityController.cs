using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityModelExtras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace WizardAppApi.Controllers
{
    public class ClaimHandle
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [DataContract]
    public class BindModel
    {
        [DataMember(Name = "id_token")]
        public string IdToken { get; set; }
    }

    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private ConfiguredDiscoverCacheContainerFactory _configuredDiscoverCacheContainerFactory;
        private IMemoryCache _memoryCache;
        private ProviderValidator _providerValidator;
        private ConfiguredDiscoverCacheContainer _discoveryContainer;

        [Route("closed")]
        [Authorize("Daffy Duck")]
        public async Task<IEnumerable<ClaimHandle>> GetClosedAsync()
        {
            if (Request.HttpContext.User != null)
            {
                var query = from item in Request.HttpContext.User.Claims
                    let c = new ClaimHandle() {Name = item.Type, Value = item.Value}
                    select c;
                return query;
            }

            return null;
        }

        public IdentityController(ConfiguredDiscoverCacheContainerFactory configuredDiscoverCacheContainerFactory,
            IMemoryCache memoryCache)
        {
            _configuredDiscoverCacheContainerFactory = configuredDiscoverCacheContainerFactory;
           _discoveryContainer = _configuredDiscoverCacheContainerFactory.Get("p7identityserver4");
            _memoryCache = memoryCache;
            _providerValidator = new ProviderValidator(_discoveryContainer, _memoryCache);

        }

        // GET api/values
        [HttpGet]
        [Route("open")]
        public async Task<IEnumerable<ClaimHandle>> GetOpenAsync()
        {
            if (Request.HttpContext.User != null)
            {
                var query = from item in Request.HttpContext.User.Claims
                    let c = new ClaimHandle() {Name = item.Type, Value = item.Value}
                    select c;
                return query;
            }

            return null;
        } // GET api/values

        [HttpPost]
        [Route("bind")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<object> PostBindAsync([FromForm] IFormCollection formCollection)
        {
            try
            {
                var idToken = formCollection["id_token"];
                var principal = await _providerValidator.ValidateToken(idToken, new TokenValidationParameters()
                {
                    ValidateAudience = false
                });
                var query = from item in principal.Claims
                    where item.Type == ClaimTypes.NameIdentifier
                    select item;
                var namedIdentifierClaim = query.FirstOrDefault();
                  var discoveryResponse = await _discoveryContainer.DiscoveryCache.GetAsync();
                var clientId = "arbitrary-resource-owner-client";
                var client = new TokenClient(
                    discoveryResponse.Issuer + "/connect/token",
                    clientId);
                Dictionary<string, string> paramaters = new Dictionary<string, string>()
                {
                    {OidcConstants.TokenRequest.ClientId, clientId},
                    {OidcConstants.TokenRequest.ClientSecret, "secret"},
                    {OidcConstants.TokenRequest.GrantType, "arbitrary_resource_owner"},
                    {
                        OidcConstants.TokenRequest.Scope,"$webappapi"
                    },
                    {
                       "arbitrary_claims",
                        "{'role': ['application', 'limited']}"
                    },
                    {
                        "subject",namedIdentifierClaim.Value
                    },
                    {"access_token_lifetime", "3600"}
                };
                var result = await client.RequestAsync(paramaters);
                
                return result.Json;
            }
            catch (Exception e)
            {
                return e.Message;
            }
          
        }
    }
}


 
