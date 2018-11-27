using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
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
    class TimeStamp
    {
        public string UtcTime { get; set; }
    }
    public class KeyVaultCache : IKeyVaultCache
    {
        private const string CacheValidationKey = "62777f80-2be6-4882-b1cc-28a202d423e6";
        private const string CacheValidationKeyTimeStamp = "104895ef-71cc-4c98-9a72-d3ffea75977b";
        private readonly IPublicKeyProvider _publicKeyProvider;
        private readonly AzureKeyVaultTokenSigningServiceOptions _keyVaultOptions;
        private readonly AzureKeyVaultAuthentication _azureKeyVaultAuthentication;
        private List<KeyBundle> _keyBundles;
        private IMemoryCache _cache;
        private ILogger _logger;
        private readonly DefaultCache<CacheData> _cachedData;
        private DefaultCache<TimeStamp> _cacheTimeStamp;

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
            _cacheTimeStamp = new DefaultCache<TimeStamp>(_cache);
        }

        public async Task<DateTime?> GetKeyVaultCacheDataUtcAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var timeStamp = await _cacheTimeStamp.GetAsync(CacheValidationKeyTimeStamp);
            if (timeStamp == null)
                return null;
            DateTime nowUtc = DateTime.ParseExact(timeStamp.UtcTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return nowUtc;
        }

        public async Task<CacheData> GetKeyVaultCacheDataAsync(CancellationToken cancellationToken)
        {
            var cachedData = await _cachedData.GetAsync(CacheValidationKey);
            if (cachedData == null)
            {
                await RefreshCacheData();
                cachedData = await _cachedData.GetAsync(CacheValidationKey);
                // TODO: need to look into this more. We can't do our scheduler scheme for this.
                // Probably should be an entry in REDIS with a timeout for refresh.
            }
            return cachedData;
        }

        public async Task RefreshCacheFromSourceAsync(CancellationToken cancellationToken)
        {
            await RefreshCacheData();
        }

        private async Task RefreshCacheData()
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime cacheTime = DateTime.UtcNow.AddHours(-7);

                var timeStamp = await _cacheTimeStamp.GetAsync(CacheValidationKeyTimeStamp);
                if (timeStamp != null)
                {
                    cacheTime = DateTime.ParseExact(timeStamp.UtcTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }

                if (now < cacheTime.AddHours(6))
                    return;

                var keyBundles = await GetKeyBundleVersionsAsync();
                var queryKbs = from item in keyBundles
                               where item.Attributes.Enabled != null && (bool)item.Attributes.Enabled
                               select item;
                keyBundles = queryKbs.ToList();

                var certificateBundle = await GetSigningCertificateAsync();
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

                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
                var tokenCreateSigningCredentials = await GetTokenCreationSigningCredentialsAsync();
                CacheData cacheData = new CacheData()
                {
                    RsaSecurityKeys = queryRsaSecurityKeys.ToList(),
                    SigningCredentials = signingCredentials,
                    JsonWebKeys = jwks,
                    KeyIdentifier = kid,
                    CertificateBundle = certificateBundle
                };
                await _cachedData.SetAsync(CacheValidationKey, cacheData, TimeSpan.FromHours(6));

                await _cacheTimeStamp.SetAsync(CacheValidationKeyTimeStamp, new TimeStamp()
                {
                    UtcTime = DateTime.UtcNow.ToString("O")
                }, TimeSpan.FromHours(6));
                
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"KeyVault RefreshCacheData fatal exception");
                throw;
            }

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
        private async Task<CertificateBundle> GetSigningCertificateAsync()
        {
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);

            var certificate = await keyVaultClient.GetCertificateAsync(_keyVaultOptions.KeyVaultUrl,_keyVaultOptions.KeyIdentifier);
             
            return certificate;
        }
    }
}