using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    public class TokenResponseGeneratorHook : TokenResponseGenerator, ITokenResponseGeneratorHookHost
    {
        private IEnumerable<ITokenResponseGeneratorHook> _tokenResponseGeneratorHooks;

        public TokenResponseGeneratorHook(
            IEnumerable<ITokenResponseGeneratorHook> tokenResponseGeneratorHooks,
            ISystemClock clock, 
            ITokenService tokenService, 
            IRefreshTokenService refreshTokenService, 
            IResourceStore resources, 
            IClientStore clients, 
            ILogger<TokenResponseGeneratorHook> logger) : base(clock, tokenService, refreshTokenService, resources, clients, logger)
        {
            _tokenResponseGeneratorHooks = tokenResponseGeneratorHooks;
        }

        protected override async Task<TokenResponse> ProcessTokenRequestAsync(
            TokenRequestValidationResult request)
        {
            foreach (var tokenResponseGeneratorHook in _tokenResponseGeneratorHooks)
            {
                var response = await tokenResponseGeneratorHook.ProcessAsync(this, request);
                if (response != null)
                {
                    return response;
                }
            }
            return await base.ProcessTokenRequestAsync(request);
        }

        public async Task<(string accessToken, string refreshToken)> CreateAccessTokenAsync(ValidatedTokenRequest request)
        {
            return await base.CreateAccessTokenAsync(request);
        }
    }
}
