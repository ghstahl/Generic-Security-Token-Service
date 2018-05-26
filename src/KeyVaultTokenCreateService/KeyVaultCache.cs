using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    public class KeyVaultCache : IKeyVaultCache
    {
        private const string CacheValidationKey = "62777f80-2be6-4882-b1cc-28a202d423e6";
        private readonly IPublicKeyProvider _publicKeyProvider;
        private readonly AzureKeyVaultTokenSigningServiceOptions _keyVaultOptions;
        private readonly AzureKeyVaultAuthentication _azureKeyVaultAuthentication;
        private List<KeyBundle> _keyBundles;
        private IMemoryCache _cache;
        private ILogger _logger;
        private readonly DefaultCache<CacheData> _cachedData;
        public KeyVaultCache(
            IPublicKeyProvider publicKeyProvider,
            IOptions<AzureKeyVaultTokenSigningServiceOptions> keyVaultOptions,
            IMemoryCache cache,
            ILogger<KeyVaultCache> logger)
        {
            _publicKeyProvider = publicKeyProvider;
            _keyVaultOptions = keyVaultOptions.Value;
            _azureKeyVaultAuthentication = new AzureKeyVaultAuthentication(_keyVaultOptions.ClientId, _keyVaultOptions.ClientSecret);
            _cache = cache;
            _logger = logger;
            _cachedData = new DefaultCache<CacheData>(_cache);
        }
        public async Task<CacheData> GetKeyVaultCacheDataAsync()
        {
            var cachedData = await _cachedData.GetAsync(CacheValidationKey) ?? await RefreshCacheData();
            return cachedData;
        }
        private async Task<CacheData> RefreshCacheData()
        {
            var keyBundles = await GetKeyBundleVersionsAsync();
            var queryKbs = from item in keyBundles
                where item.Attributes.Enabled != null && (bool)item.Attributes.Enabled
                select item;
            keyBundles = queryKbs.ToList();

            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);
            var queryRsaSecurityKeys = from item in keyBundles
                let c = new RsaSecurityKey(keyVaultClient.ToRSA(item))
                select c;

       //     var currentKeyBundle = await _publicKeyProvider.GetKeyBundleAsync();
       //     var securityKey = new RsaSecurityKey(keyVaultClient.ToRSA(currentKeyBundle));
       //     var signingCredentials = new SigningCredentials(securityKey, securityKey.Rsa.SignatureAlgorithm);

            var jwks = new List<JsonWebKey>();
            var query = from item in keyBundles
                where item.Attributes.Enabled != null && (bool)item.Attributes.Enabled
                select item;
            _keyBundles = query.ToList();
            foreach (var keyBundle in _keyBundles)
            {
                jwks.Add(new JsonWebKey(keyBundle.Key.ToString()));
            }

            var kid = await _publicKeyProvider.GetKeyIdentifierAsync();

            var jwk = await _publicKeyProvider.GetAsync();
            var parameters = new RSAParameters
            {
                Exponent = Base64UrlEncoder.DecodeBytes(jwk.E),
                Modulus = Base64UrlEncoder.DecodeBytes(jwk.N)
            };
            var securityKey = new RsaSecurityKey(parameters)
            {
                KeyId = jwk.Kid,
            };

            var signingCredentials =  new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            var tokenCreateSigningCredentials = await GetTokenCreationSigningCredentialsAsync();
            CacheData cacheData = new CacheData()
            {
                RsaSecurityKeys = queryRsaSecurityKeys.ToList(),
                SigningCredentials = signingCredentials,
                JsonWebKeys = jwks,
                KeyIdentifier = kid
            };
            await _cachedData.SetAsync(CacheValidationKey, cacheData, TimeSpan.FromMinutes(5));
            return cacheData;
        }

        private async Task<SigningCredentials> GetTokenCreationSigningCredentialsAsync()
        {
            var jwk = await _publicKeyProvider.GetAsync();
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

        private async Task<List<KeyBundle>> GetKeyBundleVersionsAsync()
        {
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);

            List<KeyItem> keyItems = new List<KeyItem>();

            var page = await keyVaultClient.GetKeyVersionsAsync(_keyVaultOptions.KeyVaultUrl, _keyVaultOptions.KeyIdentifier);
            keyItems.AddRange(page);
            while (!string.IsNullOrWhiteSpace(page.NextPageLink))
            {
                page = await keyVaultClient.GetKeyVersionsNextAsync(page.NextPageLink);
                keyItems.AddRange(page);

            }
            var keyBundles = new List<KeyBundle>();

            foreach (var keyItem in keyItems)
            {
                var keyBundle = await keyVaultClient.GetKeyAsync(keyItem.Identifier.Identifier);
                keyBundles.Add(keyBundle);
            }

            return keyBundles;
        }
    }
}