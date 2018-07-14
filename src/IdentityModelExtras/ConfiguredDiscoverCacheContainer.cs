using System.Collections.Generic;
using System.Linq;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace IdentityModelExtras
{
    /*
     "oauth2": [
        {
          "scheme": "norton",
          "authority": "https://login-int.norton.com/sso/oidc1/token",
          "callbackPath": "/signin-norton",
          "additionalEndpointBaseAddresses": [
            "https://login-int.norton.com/sso/idp/OIDC",
            "https://login-int.norton.com/sso/oidc1"
          ]
        },  
        {
          "scheme": "google",
          "authority": "https://accounts.google.com",
          "callbackPath": "/signin-google",
          "additionalEndpointBaseAddresses": []
        }
      ]
   */

    public class OAuth2SchemeRecord
    {
        public string Scheme { get; set; }
        public string ClientId { get; set; }
        public string Authority { get; set; }
        public string CallbackPath { get; set; }
        public List<string> AdditionalEndpointBaseAddresses { get; set; }
    }

    public class ConfiguredDiscoverCacheContainer : IDiscoveryCacheContainer
    {
        private IConfiguration _configuration;
        private IDefaultHttpClientFactory _defaultHttpClientFactory;
        private DiscoveryCache _discoveryCache { get; set; }
        private string Scheme { get; set; }
        private List<OAuth2SchemeRecord> OAuth2SchemeRecords { get; set; }
        public ConfiguredDiscoverCacheContainer(
            IDefaultHttpClientFactory defaultHttpClientFactory, 
            IConfiguration configuration, 
            string scheme)
        {
            _defaultHttpClientFactory = defaultHttpClientFactory;
            _configuration = configuration;
            var section = configuration.GetSection("oauth2");
            OAuth2SchemeRecords = new List<OAuth2SchemeRecord>();
            section.Bind(OAuth2SchemeRecords);
            Scheme = scheme;
        }
        public  DiscoveryCache DiscoveryCache
        {
            get
            {
                if (_discoveryCache == null)
                {
                    var query = from item in OAuth2SchemeRecords
                                where item.Scheme == Scheme
                                select item;
                    var record = query.FirstOrDefault();

                    DiscoveryPolicy discoveryPolicy = null;
                    if (record.AdditionalEndpointBaseAddresses != null && record.AdditionalEndpointBaseAddresses.Any())
                    {
                        discoveryPolicy = new DiscoveryPolicy();
                        foreach (var additionalEndpointBaseAddress in record.AdditionalEndpointBaseAddresses)
                        {
                            discoveryPolicy.AdditionalEndpointBaseAddresses.Add(additionalEndpointBaseAddress);
                        }
                    }
                    _discoveryCache = new DiscoveryCache(
                        record.Authority, 
                        _defaultHttpClientFactory.HttpClient,
                        discoveryPolicy);
                }
                return _discoveryCache;
            }
        }
    }

    public class ConfiguredDiscoverCacheContainerFactory
    {
        private IConfiguration _configuration;
        private IDefaultHttpClientFactory _defaultHttpClientFactory;
        private Dictionary<string, ConfiguredDiscoverCacheContainer> OIDCDiscoverCacheContainers { get; set; }
        private List<OAuth2SchemeRecord> OAuth2SchemeRecords { get; set; }

        public ConfiguredDiscoverCacheContainerFactory(IDefaultHttpClientFactory defaultHttpClientFactory, 
            IConfiguration configuration)
        {
            _defaultHttpClientFactory = defaultHttpClientFactory;
            _configuration = configuration;
            var section = configuration.GetSection("oauth2");
            OAuth2SchemeRecords = new List<OAuth2SchemeRecord>();
            section.Bind(OAuth2SchemeRecords);
            OIDCDiscoverCacheContainers = new Dictionary<string, ConfiguredDiscoverCacheContainer>();
            foreach (var record in OAuth2SchemeRecords)
            {
                OIDCDiscoverCacheContainers.Add(record.Scheme, 
                    new ConfiguredDiscoverCacheContainer(_defaultHttpClientFactory,_configuration, record.Scheme));
            }
        }



        public ConfiguredDiscoverCacheContainer Get(string scheme)
        {
            if (OIDCDiscoverCacheContainers.ContainsKey(scheme))
            {
                return OIDCDiscoverCacheContainers[scheme];
            }
            return null;
        }
    }
}