using System;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public class ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => Constants.ArbitraryOIDCResourceOwner;

        private readonly ILogger<ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator> _logger;
        private readonly IEventService _events;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IRawClientSecretValidator _clientSecretValidator;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ISystemClock _clock;
        private ArbitraryOpenIdConnectIdentityTokenRequestValidator _arbitraryOpenIdConnectIdentityTokenRequestValidator;

        public ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator(
            IdentityServerOptions options,
            IClientStore clientStore,
            IRawClientSecretValidator clientSecretValidator,
            IResourceStore resourceStore,
            IEventService events,
            ISystemClock clock,
            ITokenResponseGenerator tokenResponseGenerator,
            ILogger<ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator> logger,
            ArbitraryOpenIdConnectIdentityTokenRequestValidator arbitraryOpenIdConnectIdentityTokenRequestValidator)
        {
            _logger = logger;
            _clock = clock;
            _events = events;
            _clientSecretValidator = clientSecretValidator;
            _options = options;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _tokenResponseGenerator = tokenResponseGenerator;
            _arbitraryOpenIdConnectIdentityTokenRequestValidator = arbitraryOpenIdConnectIdentityTokenRequestValidator;
        }

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new NotImplementedException();
        }

    }
}
