using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public class ProviderValidatorManager
    {
        private ILogger<ProviderValidatorManager> _logger;
        private IMemoryCache _cache;

        private Dictionary<string, ProviderValidator> _providerValidators =>
            new Dictionary<string, ProviderValidator>();

        public ProviderValidatorManager(ILogger<ProviderValidatorManager> logger,
            IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public ProviderValidator FetchProviderValidator(string wellknownAuthority)
        {
            if (WellknownAuthorities.AuthoritiesMap.ContainsKey(wellknownAuthority))
            {
                if (_providerValidators.ContainsKey(wellknownAuthority))
                    return _providerValidators[wellknownAuthority];
                try
                {
                    var container = new OpenIdConnectDiscoverCacheContainer(WellknownAuthorities.AuthoritiesMap[wellknownAuthority]);
                    container.DiscoveryCache.Refresh();
                    var providerValidator = new ProviderValidator(container, _cache);
                    _providerValidators.Add(wellknownAuthority, providerValidator);
                    return providerValidator;
                }
                catch (Exception e)
                {
                    //TODO: log and permanently mark this wellknown as bad

                }
            }
            return null;
        }
    }
}