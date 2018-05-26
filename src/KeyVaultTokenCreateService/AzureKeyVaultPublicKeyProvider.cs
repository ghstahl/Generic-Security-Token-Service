using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JsonWebKey = IdentityModel.Jwk.JsonWebKey;

namespace P7IdentityServer4
{
    public class AzureKeyVaultPublicKeyProvider : IPublicKeyProvider
    {
        private readonly AzureKeyVaultTokenSigningServiceOptions _options;
        private readonly AzureKeyVaultAuthentication _authentication;
        private List<JsonWebKey> _jwks;
        private JsonWebKey _jwk;
        private KeyIdentifier _keyIdentifier;
        private KeyBundle _keyBundle;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKeyVaultTokenSigningService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AzureKeyVaultPublicKeyProvider(IOptions<AzureKeyVaultTokenSigningServiceOptions> options):this(options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKeyVaultTokenSigningService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AzureKeyVaultPublicKeyProvider(AzureKeyVaultTokenSigningServiceOptions options)
        {
            _options = options;
            _authentication = new AzureKeyVaultAuthentication(_options.ClientId, _options.ClientSecret);
        }

        public async Task<IEnumerable<JsonWebKey>> GetAllAsync()
        {
            if (_jwks == null)
            {
                _jwks = new List<JsonWebKey>();
                var keyBundles = await GetKeyBundleVersionsAsync();
                var query = from item in keyBundles
                    where item.Attributes.Enabled != null && (bool) item.Attributes.Enabled
                    select item;
                keyBundles = query.ToList();
                foreach (var keyBundle in keyBundles)
                {
                    var jwk = new JsonWebKey(keyBundle.Key.ToString());
                    _jwks.Add(jwk);
                }
            }
            return _jwks;
        }

        public async Task<KeyBundle> GetKeyBundleAsync()
        {
            await GetAsync();
            return _keyBundle;
        }

        public async Task<KeyIdentifier> GetKeyIdentifierAsync()
        {
            if (_keyIdentifier == null)
            {
                await GetAsync();
            }
            return _keyIdentifier;
        }

        public AzureKeyVaultTokenSigningServiceOptions GetProviderSettings()
        {
            return _options;
        }

        private async Task<KeyBundle> GetSigningKeyAsync()
        {
            var keyVaultClient = new KeyVaultClient(_authentication.KeyVaultClientAuthenticationCallback);

            var keyBundle = await keyVaultClient.GetKeyAsync(_options.KeyVaultUrl, _options.KeyIdentifier);
            if (keyBundle == null)
            {
                throw new InvalidOperationException("Failed to get the signing key bundle from Azure Key Vault");
            }

            return keyBundle;
        }
        private async Task<List<KeyBundle>> GetKeyBundleVersionsAsync()
        {
            var keyVaultClient = new KeyVaultClient(_authentication.KeyVaultClientAuthenticationCallback);

            List<KeyItem> keyItems = new List<KeyItem>();

            var page = await keyVaultClient.GetKeyVersionsAsync(_options.KeyVaultUrl, _options.KeyIdentifier);
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

        public async Task<JsonWebKey> GetAsync()
        {
            if (_jwk == null)
            {
                var keyBundle = await GetSigningKeyAsync();
                _keyBundle = keyBundle;
                _keyIdentifier = keyBundle.KeyIdentifier;
                _jwk = new JsonWebKey(keyBundle.Key.ToString());
            }
            return _jwk;
        }
    }
}