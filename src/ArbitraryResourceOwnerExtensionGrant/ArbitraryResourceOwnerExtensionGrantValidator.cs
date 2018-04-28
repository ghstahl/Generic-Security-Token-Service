using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class ArbitraryResourceOwnerExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryResourceOwnerExtensionGrantValidator> _logger;
        private readonly IEventService _events;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IRawClientSecretValidator _clientSecretValidator;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ISystemClock _clock;
        private ArbitraryResourceOwnerRequestValidator _arbitraryResourceOwnerRequestValidator;

        public ArbitraryResourceOwnerExtensionGrantValidator(
            IdentityServerOptions options,
            IClientStore clientStore,
            IRawClientSecretValidator clientSecretValidator,
            IResourceStore resourceStore,
            IEventService events,
            ISystemClock clock,
            ITokenResponseGenerator tokenResponseGenerator,
            ILogger<ArbitraryResourceOwnerExtensionGrantValidator> logger,
            ArbitraryResourceOwnerRequestValidator arbitraryResourceOwnerRequestValidator)
        {
            _logger = logger;
            _clock = clock;
            _events = events;
            _clientSecretValidator = clientSecretValidator;
            _options = options;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _tokenResponseGenerator = tokenResponseGenerator;
            _arbitraryResourceOwnerRequestValidator = arbitraryResourceOwnerRequestValidator;
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
            await _arbitraryResourceOwnerRequestValidator.ValidateAsync(customTokenRequestValidationContext);
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

            _validatedRequest.GrantType = grantType;
            var resource = await _resourceStore.GetAllResourcesAsync();
            // get user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, context.Request.Raw.Get("subject")),
                new Claim("sub", context.Request.Raw.Get("subject"))
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            AugmentPrincipal(principal);
            var userClaimsFinal = new List<Claim>()
            {
                new Claim(ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims, raw[ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims])
            };
            context.Result = new GrantValidationResult(principal.GetSubjectId(), ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner, userClaimsFinal);
        }
        private void AugmentPrincipal(ClaimsPrincipal principal)
        {
            _logger.LogDebug("Augmenting SignInContext");
            AugmentMissingClaims(principal, _clock.UtcNow.UtcDateTime);
        }
        private void AugmentMissingClaims(ClaimsPrincipal principal, DateTime authTime)
        {
            var identity = principal.Identities.First();

            // ASP.NET Identity issues this claim type and uses the authentication middleware name
            // such as "Google" for the value. this code is trying to correct/convert that for
            // our scenario. IOW, we take their old AuthenticationMethod value of "Google"
            // and issue it as the idp claim. we then also issue a amr with "external"
            var amr = identity.FindFirst(ClaimTypes.AuthenticationMethod);
            if (amr != null &&
                identity.FindFirst(JwtClaimTypes.IdentityProvider) == null &&
                identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
            {
                _logger.LogDebug("Removing amr claim with value: {value}", amr.Value);
                identity.RemoveClaim(amr);

                _logger.LogDebug("Adding idp claim with value: {value}", amr.Value);
                identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, amr.Value));

                _logger.LogDebug("Adding amr claim with value: {value}",  Constants.ExternalAuthenticationMethod);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, Constants.ExternalAuthenticationMethod));
            }

            if (identity.FindFirst(JwtClaimTypes.IdentityProvider) == null)
            {
                _logger.LogDebug("Adding idp claim with value: {value}", IdentityServerConstants.LocalIdentityProvider);
                identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider));
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationMethod) == null)
            {
                if (identity.FindFirst(JwtClaimTypes.IdentityProvider).Value == IdentityServerConstants.LocalIdentityProvider)
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", OidcConstants.AuthenticationMethods.Password);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password));
                }
                else
                {
                    _logger.LogDebug("Adding amr claim with value: {value}", Constants.ExternalAuthenticationMethod);
                    identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, Constants.ExternalAuthenticationMethod));
                }
            }

            if (identity.FindFirst(JwtClaimTypes.AuthenticationTime) == null)
            {
                var time = authTime.ToEpochTime().ToString();

                _logger.LogDebug("Adding auth_time claim with value: {value}", time);
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationTime, time, ClaimValueTypes.Integer));
            }
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
        public string GrantType => Constants.ArbitraryResourceOwner;
    }
}
