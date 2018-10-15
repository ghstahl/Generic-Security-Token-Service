using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    /// <summary>
    /// Default refresh token service
    /// </summary>
    public class DefaultRefreshTokenServiceExtra : IRefreshTokenService
    {
        private IRefreshTokenService _refreshTokenService;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRefreshTokenService" /> class.
        /// </summary>
        /// <param name="clock">The clock</param>
        /// <param name="refreshTokenStore">The refresh token store</param>
        /// <param name="logger">The logger</param>
        public DefaultRefreshTokenServiceExtra(
            DefaultRefreshTokenService defaultRefreshTokenService, 
            ILogger<DefaultRefreshTokenService> logger)
        {
            _refreshTokenService = defaultRefreshTokenService;
            _logger = logger;
        }

        /// <summary>
        /// Creates the refresh token.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// The refresh token handle
        /// </returns>
        public virtual async Task<string> CreateRefreshTokenAsync(ClaimsPrincipal subject, Token accessToken, Client client)
        {
            var handle =  await _refreshTokenService.CreateRefreshTokenAsync(subject, accessToken, client);
            // protect the handle 
            return handle;
        }

        /// <summary>
        /// Updates the refresh token.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// The refresh token handle
        /// </returns>
        public virtual async Task<string> UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken, Client client)
        {
            // unprotect the handle
            var newHandle = await _refreshTokenService.UpdateRefreshTokenAsync(handle, refreshToken, client);
            // protect handle
            return newHandle;
        }
    }
}
