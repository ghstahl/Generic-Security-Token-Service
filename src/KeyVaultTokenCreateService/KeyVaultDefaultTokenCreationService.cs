using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    public class KeyVaultDefaultTokenCreationService: DefaultTokenCreationService
    {
        public KeyVaultDefaultTokenCreationService(ISystemClock clock, IKeyMaterialService keys, ILogger<DefaultTokenCreationService> logger) : base(clock, keys, logger)
        {
        }
        protected override Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            var rawDataBytes = System.Text.Encoding.UTF8.GetBytes(jwt.EncodedHeader + "." + jwt.EncodedPayload);

            /** KeyVault keys' KeyIdentifierFormat: https://{vaultname}.vault.azure.net/keys/{keyname} */
            var keyIdentifier = string.Format(KeyVaultConstants.KeyIdentifierFormat, settings.VaultName, settings.KeyName);
            var signatureProvider = new KeyVaultSignatureProvider(keyIdentifier, JsonWebKeySignatureAlgorithm.RS256, authentication.KeyVaultAuthenticationCallback);

            var rawSignature = await Task.Run(() => Base64UrlEncoder.Encode(signatureProvider.Sign(rawDataBytes))).ConfigureAwait(false);

            return jwt.EncodedHeader + "." + jwt.EncodedPayload + "." + rawSignature;
        }

        protected override async Task<JwtHeader> CreateHeaderAsync(Token token)
        {
            var credentials = await GetSigningCredentialsAsync();
            var header = new JwtHeader(credentials);
            return header;
        }

        private async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var jwk = await publicKeyProvider.GetSigningCredentialsAsync();
            var parameters = new RSAParameters
            {
                Exponent = Base64UrlEncoder.DecodeBytes(jwk.E),
                Modulus = Base64UrlEncoder.DecodeBytes(jwk.N)
            };
            var securityKey = new RsaSecurityKey(parameters)
            {
                KeyId = jwk.Kid,
            };

            return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        }
    }
}
