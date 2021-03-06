﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ArbitraryOpenIdConnectTokenExtensionGrants.Extensions;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
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
        private PrincipalAugmenter _principalAugmenter;
        private ProviderValidatorManager _providerValidatorManager;
        private IMemoryCache _cache;
        public ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator(
            IdentityServerOptions options,
            IClientStore clientStore,
            IRawClientSecretValidator clientSecretValidator,
            IResourceStore resourceStore,
            IEventService events,
            ISystemClock clock,
            ITokenResponseGenerator tokenResponseGenerator,
            ILogger<ArbitraryOpenIdConnectIdentityTokenExtensionGrantValidator> logger,
            ArbitraryOpenIdConnectIdentityTokenRequestValidator arbitraryOpenIdConnectIdentityTokenRequestValidator,
            PrincipalAugmenter principalAugmenter,
            ProviderValidatorManager providerValidatorManager,
            IMemoryCache cache)
        {
            _cache = cache;
            _logger = logger;
            _clock = clock;
            _events = events;
            _clientSecretValidator = clientSecretValidator;
            _options = options;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _tokenResponseGenerator = tokenResponseGenerator;
            _arbitraryOpenIdConnectIdentityTokenRequestValidator = arbitraryOpenIdConnectIdentityTokenRequestValidator;
            _principalAugmenter = principalAugmenter;
            _providerValidatorManager = providerValidatorManager;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");

            if (context == null) throw new ArgumentNullException(nameof(context));
            var raw = context.Request.Raw;
            _validatedRequest = new ValidatedTokenRequest
            {
                Raw = raw ?? throw new ArgumentNullException(nameof(raw)),
                Options = _options
            };
            var customTokenRequestValidationContext = new CustomTokenRequestValidationContext()
            {
                Result = new TokenRequestValidationResult(_validatedRequest)
            };
            await _arbitraryOpenIdConnectIdentityTokenRequestValidator.ValidateAsync(customTokenRequestValidationContext);
            if (customTokenRequestValidationContext.Result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    customTokenRequestValidationContext.Result.Error);
                return;
            }
            var clientValidationResult = await _clientSecretValidator.ValidateAsync(_validatedRequest.Raw);
            if (clientValidationResult == null) throw new ArgumentNullException(nameof(clientValidationResult));

            _validatedRequest.SetClient(clientValidationResult.Client, clientValidationResult.Secret);

            /////////////////////////////////////////////
            // check grant type
            /////////////////////////////////////////////
            var grantType = _validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType.IsMissing())
            {
                LogError("Grant type is missing");
                context.Result = new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
                return;
            }
            if (grantType.Length > _options.InputLengthRestrictions.GrantType)
            {
                LogError("Grant type is too long");
                context.Result = new GrantValidationResult(TokenRequestErrors.UnsupportedGrantType);
                return;
            }
            // optional stuff;
            var accessTokenLifetimeOverride = _validatedRequest.Raw.Get(Constants.AccessTokenLifetime);
            if (!string.IsNullOrWhiteSpace(accessTokenLifetimeOverride))
            {
                var accessTokenLifetime = Int32.Parse(accessTokenLifetimeOverride);
                if (accessTokenLifetime > 0 && accessTokenLifetime < context.Request.AccessTokenLifetime)
                {
                    context.Request.AccessTokenLifetime = accessTokenLifetime;
                }
                else
                {
                    var errorDescription =
                        $"{Constants.AccessTokenLifetime} out of range.   Must be > 0 and less than configured AccessTokenLifetime.";
                    LogError(errorDescription);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, errorDescription);
                    return;
                }
            }

            var providerValidator = _providerValidatorManager.FetchProviderValidator(raw[Constants.Authority]);
            var idToken = raw[Constants.IdToken];
            var principal = await providerValidator.ValidateToken(idToken);

            var subjectId = principal.GetClaimValue(ClaimTypes.NameIdentifier);
            var newPrincipal = principal.AddUpdateClaim(ClaimTypes.NameIdentifier, $"myCompany.{subjectId}");
            principal = newPrincipal;

            _validatedRequest.GrantType = grantType;
            var resource = await _resourceStore.GetAllResourcesAsync();
            // get user's identity

            _principalAugmenter.AugmentPrincipal(principal);
            var userClaimsFinal = new List<Claim>()
            {
                new Claim(Constants.ArbitraryClaims, raw[Constants.ArbitraryClaims])
            };
            userClaimsFinal.AddRange(principal.Claims);
            userClaimsFinal.Add(new Claim(ProfileServiceManager.Constants.ClaimKey, Constants.ArbitraryOpenIdConnectIdTokenProfileService));

            context.Result = new GrantValidationResult(principal.GetNamedIdentifier(), Constants.ArbitraryOIDCResourceOwner, userClaimsFinal);
        }
        private void LogError(string message = null, params object[] values)
        {
            if (message.IsPresent())
            {
                try
                {
                    _logger.LogError(message, values);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error logging {exception}", ex.Message);
                }
            }

            //  var details = new global::IdentityServer4.Logging.TokenRequestValidationLog(_validatedRequest);
            //  _logger.LogError("{details}", details);
        }
    }
}
