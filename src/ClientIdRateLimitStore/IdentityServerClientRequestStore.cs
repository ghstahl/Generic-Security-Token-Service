using System.Threading.Tasks;
using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace ClientIdRateLimitStore
{
    public class IdentityServerClientRequestStore : IClientRequestStore
    {
        private SecretParser _parser;

        public IdentityServerClientRequestStore(SecretParser parser)
        {
            _parser = parser;
        }

        public async Task<ClientRequestIdentity> GetClientRequestIdentityAsync(HttpContext httpContext)
        {
            var clientId = "anon";
            var parsedSecret = await _parser.ParseAsync(httpContext);
            if (parsedSecret != null)
            {
                clientId = parsedSecret.Id;

            }

            var identity = new ClientRequestIdentity
            {
                Path = httpContext.Request.Path.ToString().ToLowerInvariant(),
                HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
                ClientId = clientId
            };
            return identity;
        }
    }
}