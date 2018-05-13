using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer4.HostApp.IDP
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
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
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
                        ArbitraryOpenIdConnectTokenExtensionGrants.Constants.ArbitraryOIDCResourceOwner,
                        ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject,
                        GrantType.Implicit
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "nitro",
                        "metal",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile },
                    RequireClientSecret = false,
                    AccessTokenLifetime = AccessTokenLifetimeMax,//this is the default if not pased in, and the upperrange.
                    AbsoluteRefreshTokenLifetime =AbsoluteRefreshTokenLifetimeMax,
                    ClientClaimsPrefix = null,
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                }
            };
        }
        
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}
