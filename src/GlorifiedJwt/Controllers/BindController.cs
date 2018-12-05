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
        private IActionContextAccessor _actionContextAccessor;
        private ISingletonAutoObjectCache<BindController, Dictionary<string, object>> _objectCache;
        private ITokenRequestValidator _requestValidator;
        private ITokenResponseGenerator _responseGenerator;
        private IClientStoreExtra _clientStore;

        private ITokenEndpointHandlerExtra _tokenEndpointHandlerExtra;

        public BindController(
            IActionContextAccessor actionContextAccessor,
            ITokenEndpointHandlerExtra tokenEndpointHandlerExtra,
            IClientStoreExtra clientStore,
            ITokenRequestValidator requestValidator,
            ITokenResponseGenerator responseGenerator,
            ISingletonAutoObjectCache<BindController, Dictionary<string, object>> objectCache,
            ILogger<BindController> logger)
        {
            _actionContextAccessor = actionContextAccessor;
            _tokenEndpointHandlerExtra = tokenEndpointHandlerExtra;
            _clientStore = clientStore;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _objectCache = objectCache;
            _logger = logger;
        }

        Dictionary<string, object> GetOutput()
        {

            var dictionaryCache = _objectCache.Value;

            if (dictionaryCache.TryGetValue("summary-output", out var result))
            {
                return result as Dictionary<string, object>;
            }

            var request = _actionContextAccessor.ActionContext.HttpContext.Request;

            var credits = new Dictionary<string, string>()
            {
                {
                    "ASP.NET Core Test Server",
                    "https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1"
                },

            };
            var summary = new Dictionary<string, object>
            {
                {"version", "1.0"},
                {"application", "AzureApiFunction"},
                {"author", "Herb Stahl"},
                {"credits", credits},
                {"authority", $"{request.Scheme}://{request.Host.Value}"}
            };

            dictionaryCache.TryAdd("summary-output", summary);
            return summary;
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
                ArbitraryAmrs = new List<string>() { "agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"},
                ArbitraryAudiences = new List<string>() { "cat","dog" },
                CustomPayload = new CustomPayload()
            };

            FormCollection formCollection = new FormCollection(new Dictionary<string, StringValues>()
            {
                {"client_id", "arbitrary-resource-owner-client"},
                {"grant_type", "arbitrary_resource_owner"},
                {"scope", "offline_access metal nitro In Flames"},
                {"subject", "PorkyPig"},
                {"arbitrary_claims", "{\"top\":[\"TopDog\"],\"role\": [\"application\",\"limited\"],\"query\": [\"dashboard\", \"licensing\"],\"seatId\": [\"8c59ec41-54f3-460b-a04e-520fc5b9973d\"],\"piid\": [\"2368d213-d06c-4c2a-a099-11c34adc3579\"]}"},
                {"access_token_lifetime", "3600"},
                {"arbitrary_amrs", "[\"agent:username:agent0@supporttech.com\",\"agent:challenge:fullSSN\",\"agent:challenge:homeZip\"]"},
                {"arbitrary_audiences",  "[\"cat\",\"dog\"]"},
                {"custom_payload", "{\"some_string\": \"data\",\"some_number\": 1234,\"some_object\": { \"some_string\": \"data\",\"some_number\": 1234},\"some_array\": [{\"a\": \"b\"},{\"b\": \"c\"}]}"}
            });
            var result = await _tokenEndpointHandlerExtra.ProcessAsync(extensionGrantRequest);
            return result;
        }
    }
}