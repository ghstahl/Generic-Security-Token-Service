// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace QuickstartIdentityServer
{
    public class MultiFactorRecord
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class ClientExtra : Client
    {}
    public class TestUserExtra : TestUser
    {
        private List<MultiFactorRecord> _multiFactorRecords;

        public List<MultiFactorRecord> MultiFactorRecords =>
            _multiFactorRecords = _multiFactorRecords ?? new List<MultiFactorRecord>();
    }
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<ClientExtra>
            {
                new ClientExtra
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1"}
                },

                // resource owner password grant client
                new ClientExtra
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1"}
                },

                // OpenID Connect implicit flow client (MVC)
                new ClientExtra
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris =
                    {
                        "https://localhost:44333/signin-oidc",

                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44333/signout-callback-oidc",

                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new ClientExtra
                {
                    ClientId = "mvc2",
                    ClientName = "MVC2 Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris =
                    {

                        "https://localhost:44343/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {

                        "https://localhost:44343/Account/SignoutCallbackOidc"
                    },
                    FrontChannelLogoutSessionRequired = true,
                    FrontChannelLogoutUri = "https://localhost:44343/Account/SignoutFrontChannel",
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }


            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUserExtra
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    },
                    MultiFactorRecords =
                    {
                        new MultiFactorRecord()
                        {
                            Question = "Favorite Place",
                            Answer = "Jail"
                        },
                        new MultiFactorRecord()
                        {
                            Question = "Favorite Food",
                            Answer = "twinkies"
                        }
                    }

                },
                new TestUserExtra
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