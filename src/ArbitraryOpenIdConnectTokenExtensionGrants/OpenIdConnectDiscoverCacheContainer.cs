using System;
using System.Collections.Generic;
using IdentityModel.Client;
using IdentityModelExtras;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{

    public class OpenIdConnectDiscoverCacheContainer : IDiscoveryCacheContainer
    {
       
        private AuthorityConfig _authorityConfig;
        private DiscoveryCache _discoveryCache { get; set; }


        public OpenIdConnectDiscoverCacheContainer(AuthorityConfig authorityConfig)
        {
            if(authorityConfig == null) throw new ArgumentNullException(nameof(authorityConfig));
            if (string.IsNullOrWhiteSpace(authorityConfig.Name)) throw new ArgumentNullException(nameof(authorityConfig.Name));
            if (string.IsNullOrWhiteSpace(authorityConfig.Authority)) throw new ArgumentNullException(nameof(authorityConfig.Authority));
            if (authorityConfig.AdditionalEndpointBaseAddresses == null)
            {
                authorityConfig.AdditionalEndpointBaseAddresses = new List<string>();
            }
            _authorityConfig = authorityConfig;
        }
        public DiscoveryCache DiscoveryCache
        {
            get
            {
                if (_discoveryCache == null)
                {
                    var discoveryClient = new DiscoveryClient(_authorityConfig.Authority) {Policy = {ValidateEndpoints = false}};
                    foreach (var additionalEndpointBaseAddress in _authorityConfig.AdditionalEndpointBaseAddresses)
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