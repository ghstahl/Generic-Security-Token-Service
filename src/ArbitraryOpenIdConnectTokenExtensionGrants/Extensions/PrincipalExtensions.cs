using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace ArbitraryOpenIdConnectTokenExtensionGrants.Extensions
{
    public static class PrincipalExtensions
    {
        public static ClaimsPrincipal AddUpdateClaim(this IPrincipal currentPrincipal,
            string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            return new ClaimsPrincipal(identity);
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim.Value;
        }
    }
}