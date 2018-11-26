using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using IdentityServer4Extras;
using Microsoft.Extensions.Logging;

namespace MultiRefreshTokenSameSubjectSameClientIdWorkAround
{
    public interface IRefreshTokenStore2 : IRefreshTokenStore
    {
        /// <summary>
        /// Removes the reference tokens.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        Task RemoveRefreshTokensAsync(string subjectId, string clientId);

        /// <summary>
        /// Removes the reference tokens.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        Task RemoveRefreshTokensAsync(string subjectId);

    }

    public class DefaultRefreshTokenStoreEnhancement : DefaultRefreshTokenStore
    {
        public DefaultRefreshTokenStoreEnhancement(
            IPersistedGrantStore store, 
            IPersistentGrantSerializer serializer, 
            IHandleGenerationService handleGenerationService, 
            ILogger<DefaultRefreshTokenStore> logger) : base(store, serializer, handleGenerationService, logger)
        {
        }
        public async Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            await RemoveAllAsync(subjectId, clientId);
        }

        public Task RemoveRefreshTokensAsync(string subjectId)
        {
            throw new System.NotImplementedException();
        }
    }
    public class ProxyDefaultRefreshTokenStoreEnhancement :  IRefreshTokenStore2
    {
        private DefaultRefreshTokenStoreEnhancement _defaultRefreshTokenStoreEnhancement;
        private IRefreshTokenKeyObfuscator _refreshTokenKeyObfuscator;

        public ProxyDefaultRefreshTokenStoreEnhancement(DefaultRefreshTokenStoreEnhancement defaultRefreshTokenStoreEnhancement,
            IRefreshTokenKeyObfuscator refreshTokenKeyObfuscator)
        {
            _defaultRefreshTokenStoreEnhancement = defaultRefreshTokenStoreEnhancement;
            _refreshTokenKeyObfuscator = refreshTokenKeyObfuscator;
        }

        public async Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            var handle = await _defaultRefreshTokenStoreEnhancement.StoreRefreshTokenAsync(refreshToken);
            handle = await _refreshTokenKeyObfuscator.ObfuscateAsync(handle);
            return handle;
        }

        public async Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            handle = await _refreshTokenKeyObfuscator.UnObfuscateAsync(handle);
            await _defaultRefreshTokenStoreEnhancement.UpdateRefreshTokenAsync(handle, refreshToken);
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            refreshTokenHandle = await _refreshTokenKeyObfuscator.UnObfuscateAsync(refreshTokenHandle);
            return await _defaultRefreshTokenStoreEnhancement.GetRefreshTokenAsync(refreshTokenHandle);
          
        }

        public async Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            refreshTokenHandle = await _refreshTokenKeyObfuscator.UnObfuscateAsync(refreshTokenHandle);
            await _defaultRefreshTokenStoreEnhancement.RemoveRefreshTokenAsync(refreshTokenHandle);
        }

        public async Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            await _defaultRefreshTokenStoreEnhancement.RemoveRefreshTokensAsync(subjectId, clientId);
        }

        public Task RemoveRefreshTokensAsync(string subjectId)
        {
            throw new System.NotImplementedException();
        }
    }
}
