using System;
using System.Collections.Generic;
using IdentityModel.Client;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public class OpenIdConnectDiscoverCacheContainer : DiscoverCacheContainer
    {
        private string _authority;
        private List<string> _additionalEndpointBaseAddresses;
        private DiscoveryCache _discoveryCache { get; set; }


        public OpenIdConnectDiscoverCacheContainer(string authority, List<string> additionalEndpointBaseAddresses)
        {
            if(string.IsNullOrWhiteSpace(authority)) throw new ArgumentNullException(nameof(authority));
            if(additionalEndpointBaseAddresses == null) throw new ArgumentNullException(nameof(additionalEndpointBaseAddresses));

            _authority = authority;
            _additionalEndpointBaseAddresses = additionalEndpointBaseAddresses;
        }
        public override DiscoveryCache DiscoveryCache
        {
            get
            {
                if (_discoveryCache == null)
                {
                    var discoveryClient = new DiscoveryClient(_authority) {Policy = {ValidateEndpoints = false}};
                    foreach (var additionalEndpointBaseAddress in _additionalEndpointBaseAddresses)
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