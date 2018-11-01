using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace GenericSecurityTokenService.Security
{
    
    public class TokenValidator : ITokenValidator
    {
        private IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private IConfiguration _configuration;

        private IConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager
        {
            get
            {
                if (_configurationManager == null)
                {
                    var issuer = _configuration["ISSUER"];

                    var documentRetriever = new HttpDocumentRetriever {RequireHttps = issuer.StartsWith("https://")};

                    _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                        $"{issuer}/.well-known/openid-configuration",
                        new OpenIdConnectConfigurationRetriever(),
                        documentRetriever
                    );
                }

                return _configurationManager;

            }
        }

        public TokenValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue value)
        {
            if (value?.Scheme != "Bearer")
            {
                return null;
            }
            var config = await ConfigurationManager.GetConfigurationAsync(CancellationToken.None);
            var audience =_configuration["AUDIENCE"];

            var validationParameter = new TokenValidationParameters()
            {
                RequireSignedTokens = true,
                ValidAudience = audience,
                ValidateAudience = false,
                ValidIssuer = config.Issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = config.SigningKeys
            };

            ClaimsPrincipal result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    result = handler.ValidateToken(value.Parameter, validationParameter, out var token);
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    ConfigurationManager.RequestRefresh();
                    tries++;
                }
                catch (SecurityTokenException e)
                {
                    return null;
                }
            }

            return result;
        }
    }

}

