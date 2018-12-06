using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModelExtras;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IdentityServerRequestTracker.Services;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class ArbitraryResourceOwnerExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryResourceOwnerExtensionGrantValidator> _logger;
        private readonly IEventService _events;
        private readonly IResourceStore _resourceStore;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ISystemClock _clock;
        private IMemoryCache _cache;
        private ArbitraryResourceOwnerRequestValidator _arbitraryResourceOwnerRequestValidator;
        private PrincipalAugmenter _principalAugmenter;

        public IdentityServerRequestRecord IdentityServerRequestRecord { get; }

        private ITokenValidator _tokenValidator;
        private IServiceProvider _serviceProvider;


        public ArbitraryResourceOwnerExtensionGrantValidator(
            ITokenValidator tokenValidator,
            IdentityServerOptions options,
            IServiceProvider serviceProvider,
            IResourceStore resourceStore,
            IEventService events,
            ISystemClock clock,
            IMemoryCache cache,
            ITokenResponseGenerator tokenResponseGenerator,
            ILogger<ArbitraryResourceOwnerExtensionGrantValidator> logger,
            ArbitraryResourceOwnerRequestValidator arbitraryResourceOwnerRequestValidator,
            PrincipalAugmenter principalAugmenter)
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
            _clock = clock;
            _cache = cache;
            _events = events;
            _options = options;
            _serviceProvider = serviceProvider;
            _resourceStore = resourceStore;
            _tokenResponseGenerator = tokenResponseGenerator;
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


            _validatedRequest.SetClient(identityServerRequestRecord.Client);

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
            var accessTokenLifetimeOverride = _validatedRequest.Raw.Get(Constants.AccessTokenLifetime);
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

            //  var details = new global::IdentityServer4.Logging.TokenRequestValidationLog(_validatedRequest);
            //  _logger.LogError("{details}", details);
        }

        public string GrantType => Constants.ArbitraryResourceOwner;
    }
}
