using System.Collections.Generic;
using IdentityModel.Client;
using IdentityModelExtras;
using Microsoft.Extensions.Configuration;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class OIDCDiscoverCacheContainer : ConfiguredDiscoverCacheContainer
    {
        public OIDCDiscoverCacheContainer(IConfiguration configuration) : base(configuration, "self")
        {
        }
    }
}
