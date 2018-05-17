using System.Collections.Generic;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace IdentityModelExtras
{
    /*
     "oauth2": {
    "norton": {
      "authority": "https://login-int.norton.com/sso/oidc1/token",
      "callbackPath": "/signin-norton",
      "additionalEndpointBaseAddresses": [
        "https://login-int.norton.com/sso/idp/OIDC",
        "https://login-int.norton.com/sso/oidc1"
      ]
    },
    "google": {
      "authority": "https://accounts.google.com",
      "callbackPath": "/signin-google",
      "additionalEndpointBaseAddresses": [

      ]
    }
  }

     */
    public class ConfiguredDiscoverCacheContainer : DiscoverCacheContainer
    {
        private IConfiguration _configuration;
        private DiscoveryCache _discoveryCache { get; set; }
        private string Scheme { get; set; }

        public ConfiguredDiscoverCacheContainer(IConfiguration configuration, string scheme)
        {
            _configuration = configuration;
            Scheme = scheme;
        }
        public override DiscoveryCache DiscoveryCache
        {
            get
            {
                if (_discoveryCache == null)
                {
                     var authority = _configuration[$"oauth2:{Scheme}:authority"];
                    var additionalEndpointBaseAddresses = new List<string>();
                    _configuration.GetSection($"oauth2:{Scheme}:additionalEndpointBaseAddresses").Bind(additionalEndpointBaseAddresses);

                    var discoveryClient = new DiscoveryClient(authority);
                    discoveryClient.Policy.ValidateEndpoints = false;
                    foreach (var additionalEndpointBaseAddress in additionalEndpointBaseAddresses)
                    {
                        discoveryClient.Policy.AdditionalEndpointBaseAddresses.Add(additionalEndpointBaseAddress);
                    }
                    _discoveryCache = new DiscoveryCache(discoveryClient);
                }
                return _discoveryCache;
            }
        }
    }
}