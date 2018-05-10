using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4Extras;

namespace IdentityServer4.HostApp
{
    public class Config
    {
        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("nitro", "nitro"),
                new ApiResource("metal", "metal")
            };
        }

        private static int AccessTokenLifetimeMax => 60 * 60 * 24 * 30;// 30 day

        private static int AbsoluteRefreshTokenLifetimeMax => 60 * 60 * 24 * 30 * 12;// 1 yearish
        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                // arbitrary resource owner grant client
                new Client
                {
                    ClientId = "arbitrary-resource-owner-client",
                    AllowedGrantTypes = new[]
                    { 
                        ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner,
                        ArbitraryOpenIdConnectTokenExtensionGrants.Constants.ArbitraryOIDCResourceOwner
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "nitro","metal" },
                    RequireClientSecret = false,
                    AccessTokenLifetime = AccessTokenLifetimeMax,//this is the default if not pased in, and the upperrange.
                    AbsoluteRefreshTokenLifetime =AbsoluteRefreshTokenLifetimeMax
                } 
            };
        }
    }
}
