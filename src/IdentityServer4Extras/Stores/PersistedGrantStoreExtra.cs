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
        private IPersistedGrantStore _persistedGrantStoreImplementation;

        public PersistedGrantStoreExtra(
            IClientStoreExtra clientStoreExtra,
            ILogger<PersistedGrantStoreExtra> logger )
        {
            _logger = logger;
            _clientStoreExtra = clientStoreExtra;
            _persistedGrantStoreImplementation = new InMemoryPersistedGrantStore();
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            return _persistedGrantStoreImplementation.StoreAsync(grant);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            return _persistedGrantStoreImplementation.GetAsync(key);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return _persistedGrantStoreImplementation.GetAllAsync(subjectId);
        }

        public Task RemoveAsync(string key)
        {
            return _persistedGrantStoreImplementation.RemoveAsync(key);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            return _persistedGrantStoreImplementation.RemoveAllAsync(subjectId, clientId);
        }

        public async Task RemoveAllAsync(string subjectId, string _, string type)
        {
            var clientIds = await _clientStoreExtra.GetAllClientIdsAsync();
            foreach (var clientId in clientIds)
            {
                _persistedGrantStoreImplementation.RemoveAllAsync(subjectId, clientId, type);
            }
        }
    }
}