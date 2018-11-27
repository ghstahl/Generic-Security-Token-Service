using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    public class MySigningCredentialStore : 
        ITokenSigningCredentialStore,
        IKeyMaterialService,
        ISigningCredentialStore
    {
        private readonly IKeyVaultCache _keyVaultCache;
        private ILogger _logger;

        public MySigningCredentialStore(
            IKeyVaultCache keyVaultCache,
            ILogger<MySigningCredentialStore> logger) 
        {
            _keyVaultCache = keyVaultCache;
            _logger = logger;
        }

        public async Task<IEnumerable<SecurityKey>> GetValidationKeysAsync()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.RsaSecurityKeys;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.SigningCredentials;
        }

        public async Task<CertificateBundle> GetCertificateBundleAsync()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.CertificateBundle;
        }

        public async Task<IEnumerable<JsonWebKey>> GetAllAsync()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.JsonWebKeys;
        }
    }
}
