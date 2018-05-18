using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.HostApp;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Tests_ExtensionGrants
{
    [TestClass]
    public class ArbitraryResourceOwnerTests
    {
        private readonly TestServer _server;
        private static string ClientId => "arbitrary-resource-owner-client";
        private static string ClientSecret => "secret";

        public ArbitraryResourceOwnerTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("UnitTest") // You can set the environment you want (development, staging, production)
                .UseStartup<Startup>());
        }
        [TestMethod]
        public async Task Mint_arbitrary_resource_owner_missing_subject_and_token()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
               
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidRequest);
        }
        [TestMethod]
        public async Task Mint_arbitrary_resource_owner_remint_with_access_token()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
            result.ExpiresIn.ShouldNotBeNull();

            // remint, but pass in the access_token from above
            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    OidcConstants.TokenTypes.AccessToken,
                    result.AccessToken
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Mint_arbitrary_resource_owner_with_offline_access()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Mint_arbitrary_resource_owner_with_no_offline_access()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };

            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Mint_arbitrary_resource_owner_and_refresh()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };

            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Mint_arbitrary_resource_owner_and_refresh_and_revoke()
        {
            /*
                grant_type:arbitrary_resource_owner
                client_id:arbitrary-resource-owner-client
                client_secret:secret
                scope:offline_access nitro metal
                arbitrary_claims:{"some-guid":"1234abcd","In":"Flames"}
                subject:Ratt
                access_token_lifetime:3600000
             */

            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            var revocationTokenClient = new TokenClient(
                _server.BaseAddress + "connect/revocation",
                ClientId,
                ClientSecret,
                _server.CreateHandler());
            paramaters = new Dictionary<string, string>()
            {
                {"token_type_hint", OidcConstants.TokenTypes.RefreshToken},
                {"token", result.RefreshToken}
            };
            await revocationTokenClient.RequestAsync(paramaters);

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidGrant);
        }
        [TestMethod]
        public async Task Mint_multi_arbitrary_resource_owner_and_refresh_and_revoke()
        {
            var tokenClient = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims,
                    "{'some-guid':'1234abcd','In':'Flames'}"
                },
                {
                    ArbitraryResourceOwnerExtensionGrant.Constants.Subject,
                    "Ratt"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await tokenClient.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            // mint a duplicate, this should be 2 refresh tokens.
            var result2 = await tokenClient.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            // first refresh
            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await tokenClient.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result2.RefreshToken}
            };
            result2 = await tokenClient.RequestAsync(paramaters);
            result2.AccessToken.ShouldNotBeNullOrEmpty();
            result2.RefreshToken.ShouldNotBeNull();
            result2.ExpiresIn.ShouldNotBeNull();

            var revocationTokenClient = new TokenClient(
                _server.BaseAddress + "connect/revocation",
                ClientId,
                ClientSecret,
                _server.CreateHandler());
            paramaters = new Dictionary<string, string>()
            {
                {"token_type_hint", OidcConstants.TokenTypes.RefreshToken},
                {"token", result.RefreshToken}
            };
            await revocationTokenClient.RequestAsync(paramaters);

            // try refreshing, these should now fail the refresh.
            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result.RefreshToken}
            };
            result = await tokenClient.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.Error.ShouldBe(OidcConstants.TokenErrors.InvalidGrant);

            paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken},
                {OidcConstants.TokenRequest.RefreshToken, result2.RefreshToken}
            };
            result2 = await tokenClient.RequestAsync(paramaters);
            result2.Error.ShouldNotBeNullOrEmpty();
            result2.Error.ShouldBe(OidcConstants.TokenErrors.InvalidGrant);
        }
    }
}
