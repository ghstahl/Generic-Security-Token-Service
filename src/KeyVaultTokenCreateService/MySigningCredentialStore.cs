using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
    class MySigningCredentials : SigningCredentials
    {
        public MySigningCredentials(X509Certificate2 certificate) : base(certificate)
        {
        }

        protected MySigningCredentials(X509Certificate2 certificate, string algorithm) : base(certificate, algorithm)
        {
        }

        public MySigningCredentials(SecurityKey key, string algorithm) : base(key, algorithm)
        {
        }

        public MySigningCredentials(SecurityKey key, string algorithm, string digest) : base(key, algorithm, digest)
        {
        }
    }
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

        public async Task<X509Certificate2> GeX509Certificate2Async()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.X509Certificate2;
        }

        public async Task<IEnumerable<JsonWebKey>> GetAllAsync()
        {
            var cachedData = await _keyVaultCache.GetKeyVaultCacheDataAsync();
            return cachedData.JsonWebKeys;
        }
    }
}
