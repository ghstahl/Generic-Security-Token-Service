using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;

namespace MultiRefreshTokenSameSubjectSameClientIdWorkAround
{
    public interface IReferenceTokenStore2
    {
        /// <summary>
        /// Removes the reference tokens.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        Task RemoveRefreshTokensAsync(string subjectId, string clientId);
    }

    public class MyDefaultRefreshTokenStore : DefaultRefreshTokenStore, IReferenceTokenStore2
    {
        public MyDefaultRefreshTokenStore(IPersistedGrantStore store, IPersistentGrantSerializer serializer, IHandleGenerationService handleGenerationService, ILogger<DefaultRefreshTokenStore> logger) : base(store, serializer, handleGenerationService, logger)
        {
        }
        public async Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            await RemoveAllAsync(subjectId, clientId);
        }
    }
}
