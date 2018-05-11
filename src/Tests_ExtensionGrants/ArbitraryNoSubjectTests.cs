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
    public class ArbitraryNoSubjectTests
    {
        private readonly TestServer _server;
        private static string ClientId => "arbitrary-resource-owner-client";
        private static string ClientSecret => "secret";

        public ArbitraryNoSubjectTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
        }

        [TestMethod]
        public async Task Mint_arbitrary_no_subject_with_offline_access()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'sub':'Ratt','some-guid':'1234abcd','In':'Flames'}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Mint_arbitrary_no_subject_with_no_offline_access()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {OidcConstants.TokenRequest.Scope, "nitro metal"},
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'sub':'Ratt','some-guid':'1234abcd','In':'Flames'}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Mint_arbitrary_no_subject_and_refresh()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'sub':'Ratt','some-guid':'1234abcd','In':'Flames'}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());
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
        public async Task Mint_arbitrary_no_subject_and_refresh_and_revoke()
        {
            var client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());

            Dictionary<string, string> paramaters = new Dictionary<string, string>()
            {
                {OidcConstants.TokenRequest.ClientId, ClientId},
                {OidcConstants.TokenRequest.ClientSecret, ClientSecret},
                {OidcConstants.TokenRequest.GrantType, ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryNoSubject},
                {
                    OidcConstants.TokenRequest.Scope,
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} nitro metal"
                },
                {
                    ArbitraryNoSubjectExtensionGrant.Constants.ArbitraryClaims,
                    "{'sub':'Ratt','some-guid':'1234abcd','In':'Flames'}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNull();
            result.ExpiresIn.ShouldNotBeNull();

            client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());
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

            client = new TokenClient(
                _server.BaseAddress + "connect/revocation",
                ClientId,
                ClientSecret,
                _server.CreateHandler());
            paramaters = new Dictionary<string, string>()
            {
                {"token_type_hint", OidcConstants.TokenTypes.RefreshToken},
                {"token", result.RefreshToken}
            };
            await client.RequestAsync(paramaters);

            client = new TokenClient(
                _server.BaseAddress + "connect/token",
                ClientId,
                _server.CreateHandler());
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
    }
}
