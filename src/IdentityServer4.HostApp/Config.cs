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
                new ApiResource("nitro", "nitro")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new ApiResource("metal", "metal"){
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<ClientExtra>
            {
                // arbitrary resource owner grant client
                new ClientExtra
                {
                    ClientId = "arbitrary-resource-owner-client",
                    AllowedGrantTypes = new[]
                    { 
                        ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "nitro","metal" },
                    RequireRefreshTokenClientSecret = false
                } 
            };
        }
    }
}
