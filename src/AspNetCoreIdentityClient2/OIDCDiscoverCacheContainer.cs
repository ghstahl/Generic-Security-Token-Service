using IdentityModelExtras;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreIdentityClient
{
    public class OIDCDiscoverCacheContainer : ConfiguredDiscoverCacheContainer
    {
        public OIDCDiscoverCacheContainer(IConfiguration configuration) : base(configuration, "oidc")
        {
        }
    }
}