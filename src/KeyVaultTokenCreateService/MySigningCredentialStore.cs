using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    public class MySigningCredentialStore : ISigningCredentialStore, IKeyMaterialService
    {
        private IPublicKeyProvider _publicKeyProvider;
        private AzureKeyVaultTokenSigningServiceOptions _keyVaultOptions;
        private List<JsonWebKey> _jwks;
        private AzureKeyVaultAuthentication _azureKeyVaultAuthentication;
        private List<KeyBundle> _keyBundles;

        public MySigningCredentialStore( 
            IPublicKeyProvider publicKeyProvider,
            IOptions<AzureKeyVaultTokenSigningServiceOptions> keyVaultOptions,
            ILogger<MySigningCredentialStore> logger) 
        {
            _publicKeyProvider = publicKeyProvider;
            _keyVaultOptions = keyVaultOptions.Value;
            _azureKeyVaultAuthentication = new AzureKeyVaultAuthentication(_keyVaultOptions.ClientId, _keyVaultOptions.ClientSecret);
        }

        public async Task<IEnumerable<SecurityKey>> GetValidationKeysAsync()
        {
            await GetAllAsync();
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);
            var query = from keyBundle in _keyBundles
                        let c = new RsaSecurityKey(keyVaultClient.ToRSA(keyBundle))
                        select c;
            return query;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var keyBundle = await _publicKeyProvider.GetKeyBundleAsync();
            var keyVaultClient = new KeyVaultClient(_azureKeyVaultAuthentication.KeyVaultClientAuthenticationCallback);
            var securityKey = new RsaSecurityKey(keyVaultClient.ToRSA(keyBundle));
            var credential = new SigningCredentials(securityKey, securityKey.Rsa.SignatureAlgorithm);
            return credential;
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
        public async Task<IEnumerable<JsonWebKey>> GetAllAsync()
        {
            if (_jwks == null)
            {
                _jwks = new List<JsonWebKey>();
                var keyBundles = await GetKeyBundleVersionsAsync();
                var query = from item in keyBundles
                    where item.Attributes.Enabled != null && (bool)item.Attributes.Enabled
                    select item;
                _keyBundles = query.ToList();
                foreach (var keyBundle in _keyBundles)
                {
                    var jwk = new JsonWebKey(keyBundle.Key.ToString());
                    _jwks.Add(jwk);
                }
            }
            return _jwks;
        }
    }
}
