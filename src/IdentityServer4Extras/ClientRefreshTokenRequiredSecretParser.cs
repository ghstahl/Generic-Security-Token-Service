using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras
{
    public class ClientRefreshTokenRequiredSecretParser : ISecretParser
    {
        private readonly ILogger _logger;
        private readonly IdentityServerOptions _options;
        private readonly IClientStoreExtra _clientStoreExtra;
        /// <summary>
        /// Creates the parser with options
        /// </summary>
        /// <param name="options">IdentityServer options</param>
        /// <param name="clientStoreExtra">The extra client store</param>
        /// <param name="logger">Logger</param>
        public ClientRefreshTokenRequiredSecretParser(
            IdentityServerOptions options, 
            IClientStoreExtra clientStoreExtra,
            ILogger<ClientRefreshTokenRequiredSecretParser> logger)
        {
            _logger = logger;
            _clientStoreExtra = clientStoreExtra;
            _options = options;
        }

        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing for secret in post body");

            if (!context.Request.HasFormContentType)
            {
                _logger.LogDebug("Content type is not a form");
                return null;
            }
            var body = await context.Request.ReadFormAsync();

            if (body != null)
            {
                var secret = body[OidcConstants.TokenRequest.ClientSecret].FirstOrDefault();

                if (secret.IsPresent())
                {
                    // some other validator better cover this.
                    return null;
                }

                var id = body[OidcConstants.TokenRequest.ClientId].FirstOrDefault();
                var refreshToken = body[OidcConstants.GrantTypes.RefreshToken].FirstOrDefault();
                var grantType = body[OidcConstants.TokenRequest.GrantType].FirstOrDefault();

                // client id must be present
                if (id.IsPresent() && refreshToken.IsPresent() && grantType.IsPresent())
                {
                    if (grantType != OidcConstants.GrantTypes.RefreshToken)
                    {
                        return null;
                    }

                    var client = await _clientStoreExtra.FindClientExtraByIdAsync(id);
                    if (client.RequireRefreshTokenClientSecret)
                    {
                        return null;
                    }

                    return new ParsedSecret
                    {
                        Id = id,
                        Credential = "success",
                        Type = AuthenticationMethod
                    };
                }
            }

            _logger.LogDebug("No secret in post body found");
            return null;
        }

        public string AuthenticationMethod => "client_refresh_token_required_secret_check";
    }
}