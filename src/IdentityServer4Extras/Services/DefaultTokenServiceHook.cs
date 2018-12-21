using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IdentityServer4Extras.Services
{
    public class DefaultTokenServiceHook : ITokenService
    {
        private DefaultTokenService _delegate;
        private IEnumerable<ITokenServiceHookPlugin> _tokenServiceHookPlugins;
        private ILogger Logger { get; }

        public DefaultTokenServiceHook(
            IEnumerable<ITokenServiceHookPlugin> tokenServiceHookPlugins,
            DefaultTokenService original,
            ILogger<DefaultTokenServiceHook> logger)
        {
            _tokenServiceHookPlugins = tokenServiceHookPlugins;
            _delegate = original;
            Logger = logger;
        }

        public async Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request)
        {
            var token = await _delegate.CreateIdentityTokenAsync(request);
            foreach (var tokenServiceHookPlugin in _tokenServiceHookPlugins)
            {
                var tokenResponse = await tokenServiceHookPlugin.OnPostCreateIdentityTokenAsync(request, token);
                if (tokenResponse.proccessed)
                {
                    token = tokenResponse.token;
                }
            }

            return token;
        }

        public async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            var token = await _delegate.CreateAccessTokenAsync(request);
            foreach (var tokenServiceHookPlugin in _tokenServiceHookPlugins)
            {
                var tokenResonse = await tokenServiceHookPlugin.OnPostCreateAccessTokenAsync(request, token);
                if (tokenResonse.proccessed)
                {
                    token = tokenResonse.token;
                }
            }

            return token;
        }

        public async Task<string> CreateSecurityTokenAsync(Token token)
        {
            return await _delegate.CreateSecurityTokenAsync(token);
        }
    }
}
