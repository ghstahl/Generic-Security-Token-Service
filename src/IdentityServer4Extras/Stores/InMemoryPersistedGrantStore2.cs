using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4Extras.Stores
{
    /// <summary>
    /// In-memory persisted grant store
    /// </summary>
    public class InMemoryPersistedGrantStore2 : IPersistedGrantStore2
    {
        private readonly ConcurrentDictionary<string, PersistedGrant> _repository = new ConcurrentDictionary<string, PersistedGrant>();

        /// <summary>
        /// Stores the grant.
        /// </summary>
        /// <param name="grant">The grant.</param>
        /// <returns></returns>
        public Task StoreAsync(PersistedGrant grant)
        {
            _repository[grant.Key] = grant;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the grant.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task<PersistedGrant> GetAsync(string key)
        {
            PersistedGrant token;
            if (_repository.TryGetValue(key, out token))
            {
                return Task.FromResult(token);
            }

            return Task.FromResult<PersistedGrant>(null);
        }

        /// <summary>
        /// Gets all grants for a given subject id.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var query =
                from item in _repository
                where item.Value.SubjectId == subjectId
                select item.Value;

            var items = query.ToArray().AsEnumerable();
            return Task.FromResult(items);
        }

        /// <summary>
        /// Removes the grant by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            _repository.TryRemove(key, out _);

            return Task.CompletedTask;
        }
        /// <summary>
        /// Removes all grants for a given subject id and client id combination.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveAllAsync(string subjectId)
        {
            var query =
                from item in _repository
                where  item.Value.SubjectId == subjectId
                select item.Key;

            var keys = query.ToArray();
            foreach (var key in keys)
            {
                _repository.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes all grants for a given subject id and client id combination.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            return RemoveAllAsync(subjectId);
            /*
            var query =
                from item in _repository
                where item.Value.ClientId == clientId &&
                      item.Value.SubjectId == subjectId
                select item.Key;

            var keys = query.ToArray();
            foreach (var key in keys)
            {
                _repository.TryRemove(key, out _);
            }

            return Task.CompletedTask;
            */
        }

        /// <summary>
        /// Removes all grants of a give type for a given subject id and client id combination.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            // we need to nuke all types on subject alone
            var query =
                from item in _repository
                where item.Value.SubjectId == subjectId &&
         //             item.Value.ClientId == clientId &&
                      item.Value.Type == type
                select item.Key;

            var keys = query.ToArray();
            foreach (var key in keys)
            {
                _repository.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }
    }
}