using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Stores
{
    public class PersistedGrantStoreExtra : IPersistedGrantStore
    {
        private IClientStoreExtra _clientStoreExtra;
        private ILogger _logger;
        private IPersistedGrantStore _inMemoryPersistedGrantStore;
        private IRefreshTokenKeyObfuscator _refreshTokenKeyObfuscator;

        public PersistedGrantStoreExtra(
            IClientStoreExtra clientStoreExtra,
            InMemoryPersistedGrantStore inMemoryPersistedGrantStore,
            IRefreshTokenKeyObfuscator refreshTokenKeyObfuscator,
            ILogger<PersistedGrantStoreExtra> logger )
        {
           
            _clientStoreExtra = clientStoreExtra;
            _inMemoryPersistedGrantStore = inMemoryPersistedGrantStore;
            _refreshTokenKeyObfuscator = refreshTokenKeyObfuscator;
            _logger = logger;
           
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            return _inMemoryPersistedGrantStore.StoreAsync(grant);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            return _inMemoryPersistedGrantStore.GetAsync(key);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return _inMemoryPersistedGrantStore.GetAllAsync(subjectId);
        }

        public Task RemoveAsync(string key)
        {
            return _inMemoryPersistedGrantStore.RemoveAsync(key);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            return _inMemoryPersistedGrantStore.RemoveAllAsync(subjectId, clientId);
        }

        public async Task RemoveAllAsync(string subjectId, string _, string type)
        {
            var clientIds = await _clientStoreExtra.GetAllClientIdsAsync();
            foreach (var clientId in clientIds)
            {
                _inMemoryPersistedGrantStore.RemoveAllAsync(subjectId, clientId, type);
            }
        }
    }
}