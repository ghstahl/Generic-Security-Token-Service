using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Models;
using Microsoft.Extensions.Logging;
using IdentityServerRequestTracker.Services;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class ArbitraryResourceOwnerExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryResourceOwnerExtensionGrantValidator> _logger;
        private IServiceProvider _serviceProvider;
        private readonly IdentityServerOptions _options;
        private ArbitraryResourceOwnerRequestValidator _arbitraryResourceOwnerRequestValidator;
        private PrincipalAugmenter _principalAugmenter;

        public IdentityServerRequestRecord IdentityServerRequestRecord { get; }

        public ArbitraryResourceOwnerExtensionGrantValidator(
            IdentityServerOptions options,
            IServiceProvider serviceProvider,
            ILogger<ArbitraryResourceOwnerExtensionGrantValidator> logger,
            ArbitraryResourceOwnerRequestValidator arbitraryResourceOwnerRequestValidator,
            PrincipalAugmenter principalAugmenter)
        {
            _logger = logger;
            _options = options;
            _serviceProvider = serviceProvider;
            _arbitraryResourceOwnerRequestValidator = arbitraryResourceOwnerRequestValidator;
            _principalAugmenter = principalAugmenter;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");

            IScopedStorage _scopedStorage = _serviceProvider.GetService(typeof(IScopedStorage)) as IScopedStorage;
            var identityServerRequestRecord =
                _scopedStorage.Storage["IdentityServerRequestRecord"] as IdentityServerRequestRecord;

            if (context == null) throw new ArgumentNullException(nameof(context));
            var raw = context.Request.Raw;
            var validatedRequest = new ValidatedTokenRequest
            {
                Raw = raw ?? throw new ArgumentNullException(nameof(raw)),
                Options = _options
            };
            var customTokenRequestValidationContext = new CustomTokenRequestValidationContext()
            {
                Result = new TokenRequestValidationResult(validatedRequest)
            };
            await _arbitraryResourceOwnerRequestValidator.ValidateAsync(customTokenRequestValidationContext);
            if (customTokenRequestValidationContext.Result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    customTokenRequestValidationContext.Result.Error);
                return;
            }


            validatedRequest.SetClient(identityServerRequestRecord.Client);

            /////////////////////////////////////////////
            // check grant type
            /////////////////////////////////////////////
            var grantType = validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);
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

            validatedRequest.GrantType = grantType;

            var subject = "";
            if (string.IsNullOrWhiteSpace(subject))
            {
                subject = context.Request.Raw.Get("subject");
            }

            // get user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, subject),
                new Claim("sub", subject)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            _principalAugmenter.AugmentPrincipal(principal);

            // optional stuff;
            var accessTokenLifetimeOverride = validatedRequest.Raw.Get(Constants.AccessTokenLifetime);
            if (!string.IsNullOrWhiteSpace(accessTokenLifetimeOverride))
            {
                var accessTokenLifetime = Int32.Parse(accessTokenLifetimeOverride);
                if (accessTokenLifetime > 0 && accessTokenLifetime <= context.Request.AccessTokenLifetime)
                {
                    context.Request.AccessTokenLifetime = accessTokenLifetime;
                }
                else
                {
                    var errorDescription =
                        $"{Constants.AccessTokenLifetime} out of range.   Must be > 0 and <= configured AccessTokenLifetime.";
                    LogError(errorDescription);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, errorDescription);
                    return;
                }
            }

            context.Result = new GrantValidationResult(principal.GetSubjectId(),
                ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner);
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
        }

        public string GrantType => Constants.ArbitraryResourceOwner;
    }
}
