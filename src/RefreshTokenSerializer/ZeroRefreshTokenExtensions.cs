using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Models;

namespace RefreshTokenSerializer
{
    public static class ZeroTokenExtensions
    {
        public static ZeroRefreshToken ToZeroRefreshToken(this RefreshToken token)
        {
            return new ZeroRefreshToken()
            {
                CreationTime = token.CreationTime,
                AccessToken = token.AccessToken.ToZeroToken(),
                Version = token.Version,
                Lifetime = token.Lifetime,
            };
        }
        public static RefreshToken ToRefreshToken(this ZeroRefreshToken zeroRefreshToken)
        {
            return new RefreshToken()
            {
                CreationTime = zeroRefreshToken.CreationTime,
                AccessToken = zeroRefreshToken.AccessToken.ToToken(),
                Version = zeroRefreshToken.Version,
                Lifetime = zeroRefreshToken.Lifetime,
            };
        }
        public static ZeroClaim ToZeroClaim(this Claim claim)
        {
            return new ZeroClaim(claim.Type,claim.Value);
        }
        public static Claim ToClaim(this ZeroClaim claim)
        {
            return new Claim(claim.Type, claim.Value);
        }
        public static List<ZeroClaim> ToZeroClaims(this IEnumerable<Claim> claims)
        {
            List<ZeroClaim> zeroClaims = new List<ZeroClaim>();
            foreach (var claim in claims)
            {
                zeroClaims.Add(claim.ToZeroClaim());
            }
            return zeroClaims;
        }
        public static List<Claim> ToClaims(this IEnumerable<ZeroClaim> zeroClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (var claim in zeroClaims)
            {
                claims.Add(claim.ToClaim());
            }
            return claims;
        }
        public static ZeroToken ToZeroToken(this Token token)
        {
            return new ZeroToken()
            {
                CreationTime = token.CreationTime,
                Version = token.Version,
                Lifetime = token.Lifetime,
                Audiences = token.Audiences.ToList(),
                AccessTokenType = token.AccessTokenType,
                Claims = token.Claims.ToZeroClaims(),
                ClientId = token.ClientId,
                Issuer = token.Issuer,
                Type = token.Type
            };
        }
        public static Token ToToken(this ZeroToken zeroToken)
        {
            return new Token()
            {
                CreationTime = zeroToken.CreationTime,
                Version = zeroToken.Version,
                Lifetime = zeroToken.Lifetime,
                Audiences = zeroToken.Audiences.ToList(),
                AccessTokenType = zeroToken.AccessTokenType,
                Claims = zeroToken.Claims.ToClaims(),
                ClientId = zeroToken.ClientId,
                Issuer = zeroToken.Issuer,
                Type = zeroToken.Type
            };
        }
    }
}