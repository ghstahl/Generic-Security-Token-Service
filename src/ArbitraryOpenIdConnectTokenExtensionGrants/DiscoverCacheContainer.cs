using IdentityModel.Client;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public abstract class DiscoverCacheContainer
    {
        public abstract DiscoveryCache DiscoveryCache { get; }
    }
}