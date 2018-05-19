using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IdentityServer4;
using RefreshTokenSerializer;
using Shouldly;
using ZeroFormatter;

namespace Tests_RefreshToken
{
    [TestClass]
    public class RefreshTokenSerializationTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var refreshToken = new RefreshToken()
            {
                AccessToken = new Token(OidcConstants.TokenTypes.AccessToken)
                {
                    CreationTime = DateTime.UtcNow,
                    Audiences = {"test-audience"},
                    Issuer = "the-issuer",
                    Lifetime = 3600,
                    Claims = new List<Claim>()
                    {
                        new Claim("a","b")
                    },
                    ClientId = "client-id",
                    AccessTokenType = AccessTokenType.Jwt
                },
                CreationTime = DateTime.UtcNow,
                Lifetime = 3600*10,
            };

            var zeroRefreshToken = refreshToken.ToZeroRefreshToken();
            var data = ZeroFormatterSerializer.Serialize<ZeroRefreshToken>(zeroRefreshToken);
            var zeroRefreshToken2 = ZeroFormatterSerializer.Deserialize<ZeroRefreshToken>(data);

            zeroRefreshToken2.ShouldBe(zeroRefreshToken);

            var refreshToken2 = zeroRefreshToken2.ToRefreshToken();


            refreshToken2.ClientId.ShouldBe(refreshToken.ClientId);
            refreshToken2.CreationTime.ShouldBe(refreshToken.CreationTime);
            refreshToken2.Lifetime.ShouldBe(refreshToken.Lifetime);
            refreshToken2.Version.ShouldBe(refreshToken.Version);
            refreshToken2.AccessToken.CreationTime.ShouldBe(refreshToken.AccessToken.CreationTime);
            refreshToken2.AccessToken.Audiences.ShouldBe(refreshToken.AccessToken.Audiences);
            refreshToken2.AccessToken.Issuer.ShouldBe(refreshToken.AccessToken.Issuer);
            refreshToken2.AccessToken.Lifetime.ShouldBe(refreshToken.AccessToken.Lifetime);
            refreshToken2.AccessToken.Claims.ShouldBe(refreshToken.AccessToken.Claims);
            refreshToken2.AccessToken.ClientId.ShouldBe(refreshToken.AccessToken.ClientId);
            refreshToken2.AccessToken.AccessTokenType.ShouldBe(refreshToken.AccessToken.AccessTokenType);

        }
    }
}
