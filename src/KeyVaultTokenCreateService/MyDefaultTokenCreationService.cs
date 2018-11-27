using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using P7.Core.Cache;

namespace P7IdentityServer4
{
    
    public class MyDefaultTokenCreationService : DefaultTokenCreationService
    {
        private readonly IKeyVaultCache _keyVaultCache;
        private IPublicKeyProvider _publicKeyProvider;
        private IOptions<AzureKeyVaultTokenSigningServiceOptions> _keyVaultOptions;
        private IMemoryCache _cache;
        private ObjectCache<AzureKeyVaultSignatureProvider> _signitureProvider;

        public MyDefaultTokenCreationService(
            IKeyVaultCache keyVaultCache,
            ISystemClock clock, 
            IKeyMaterialService keys,
            IPublicKeyProvider publicKeyProvider,
            IMemoryCache cache,
            IOptions<AzureKeyVaultTokenSigningServiceOptions> keyVaultOptions,
            ILogger<DefaultTokenCreationService> logger) : base(clock, keys, logger)
        {
            _keyVaultCache = keyVaultCache;
            _cache = cache;
            _publicKeyProvider = publicKeyProvider;
            _keyVaultOptions = keyVaultOptions;
            _signitureProvider = new ObjectCache<AzureKeyVaultSignatureProvider>(new TimeSpan(0,0,5,0),Refresher );
        }

        private async Task<AzureKeyVaultSignatureProvider> Refresher()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            var keyIdentifier = cachedData.KeyIdentifier;
            var signatureProvider = new AzureKeyVaultSignatureProvider(
                keyIdentifier.Identifier,
                JsonWebKeySignatureAlgorithm.RS256,
                new AzureKeyVaultAuthentication(_keyVaultOptions.Value.ClientId, _keyVaultOptions.Value.ClientSecret).KeyVaultClientAuthenticationCallback);
            return signatureProvider;

        }


        private async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.SigningCredentials;
        }
        protected override async Task<JwtHeader> CreateHeaderAsync(Token token)
        {
            var credentials = await GetSigningCredentialsAsync();
            var header = new JwtHeader(credentials);
            return header;
        }
        /// <summary>
        /// Applies the signature to the JWT
        /// </summary>
        /// <param name="jwt">The JWT object.</param>
        /// <returns>The signed JWT</returns>
        protected override async Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            var rawDataBytes = System.Text.Encoding.UTF8.GetBytes(jwt.EncodedHeader + "." + jwt.EncodedPayload);
            var signatureProvider = await _signitureProvider.GetValueAsync();
            var rawSignature = await Task.Run(() => Base64UrlEncoder.Encode(signatureProvider.Sign(rawDataBytes))).ConfigureAwait(false);

            return jwt.EncodedHeader + "." + jwt.EncodedPayload + "." + rawSignature;
        }
    }
}