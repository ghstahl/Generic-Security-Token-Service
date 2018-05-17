using System;
using IdentityModel.Client;

namespace IdentityModelExtras
{
    public abstract class DiscoverCacheContainer
    {
        public abstract DiscoveryCache DiscoveryCache { get; }
    }
}
