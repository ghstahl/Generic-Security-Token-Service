using IdentityModelExtras;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreIdentityClient
{
    public class OIDCDiscoverCacheContainer : ConfiguredDiscoverCacheContainer
    {
        public OIDCDiscoverCacheContainer(IDefaultHttpClientFactory defaultHttpClientFactory, IConfiguration configuration) : 
            base(defaultHttpClientFactory, configuration, "oidc")
        {
        }
    }
}