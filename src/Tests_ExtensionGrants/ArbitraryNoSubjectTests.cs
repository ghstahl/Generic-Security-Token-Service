using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.HostApp;
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

        private static string ClientId2 => "cct-client";
        private static string ClientSecret2 => "secret";

        public ArbitraryNoSubjectTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("UnitTest") // You can set the environment you want (development, staging, production)
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
                    "{'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
            result.Error.ShouldNotBeNullOrEmpty();
          
        }

        [TestMethod]
        public async Task Mint_arbitrary_no_subject_with_offline_access_invalid_claims()
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
                    "{'client_id':['d'],'sub':['dog'],'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.Error.ShouldNotBeNullOrEmpty();
            result.ErrorDescription.ShouldNotBeNullOrEmpty();
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
                    "{  'role': ['application', 'limited'],'query': ['dashboard', 'licensing'],'seatId': ['8c59ec41-54f3-460b-a04e-520fc5b9973d'],'piid': ['2368d213-d06c-4c2a-a099-11c34adc3579']}"
                },
                {ArbitraryNoSubjectExtensionGrant.Constants.AccessTokenLifetime, "3600"}
            };
            var result = await client.RequestAsync(paramaters);
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldBeNull();
            result.ExpiresIn.ShouldNotBeNull();
        }

         
 
    }
}
