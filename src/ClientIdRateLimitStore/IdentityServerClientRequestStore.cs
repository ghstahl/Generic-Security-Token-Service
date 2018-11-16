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
            var parsedSecret = await _parser.ParseAsync(httpContext);
            if (parsedSecret != null)
            {
                var identity = new ClientRequestIdentity
                {
                    Path = httpContext.Request.Path.ToString().ToLowerInvariant(),
                    HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
                    ClientId = parsedSecret.Id
                };
                return identity;
            }
            return null;
        }
    }
}