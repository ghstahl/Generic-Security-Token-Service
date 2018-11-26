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
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ArbitraryIdentityExtensionGrant
{
    public class ArbitraryIdentityExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger _logger;
        private readonly IEventService _events;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IRawClientSecretValidator _clientSecretValidator;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly IdentityServerOptions _options;
        private ValidatedTokenRequest _validatedRequest;
        private ISystemClock _clock;
        private IMemoryCache _cache;
        private ArbitraryIdentityRequestValidator _arbitraryIdentityRequestValidator;
        private PrincipalAugmenter _principalAugmenter; 
        private ITokenValidator _tokenValidator;

        public ArbitraryIdentityExtensionGrantValidator(
            ITokenValidator tokenValidator,
            IdentityServerOptions options,
            IClientStore clientStore,
            IRawClientSecretValidator clientSecretValidator,
            IResourceStore resourceStore,
            IEventService events,
            ISystemClock clock,
            IMemoryCache cache,
            ITokenResponseGenerator tokenResponseGenerator,
            ILogger<ArbitraryIdentityExtensionGrantValidator> logger,
            ArbitraryIdentityRequestValidator arbitraryIdentityRequestValidator,
            PrincipalAugmenter principalAugmenter )
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
            _clock = clock;
            _cache = cache;
            _events = events;
            _clientSecretValidator = clientSecretValidator;
            _options = options;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _tokenResponseGenerator = tokenResponseGenerator;
            _arbitraryIdentityRequestValidator = arbitraryIdentityRequestValidator;
            _principalAugmenter = principalAugmenter; 

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
            await _arbitraryIdentityRequestValidator.ValidateAsync(customTokenRequestValidationContext);
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

            var subject = "";
            Claim originAuthTimeClaim = null;
            // if access_token exists, it wins.
            var accessToken = context.Request.Raw.Get("access_token");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var validateAccessToken = await _tokenValidator.ValidateAccessTokenAsync(accessToken);
                var queryClaims = from item in validateAccessToken.Claims
                    where item.Type == JwtClaimTypes.Subject
                                  select item.Value;
                subject = queryClaims.FirstOrDefault();

                originAuthTimeClaim = (from item in validateAccessToken.Claims
                                      where item.Type == $"origin_{JwtClaimTypes.AuthenticationTime}"
                    select item).FirstOrDefault();
                if (originAuthTimeClaim == null)
                {
                    var authTimeClaim = (from item in validateAccessToken.Claims
                        where item.Type == JwtClaimTypes.AuthenticationTime
                        select item).FirstOrDefault();
                    originAuthTimeClaim = new
                        Claim($"origin_{JwtClaimTypes.AuthenticationTime}",
                            authTimeClaim.Value);

                }

            }

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
            var userClaimsFinal = new List<Claim>();

 
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
            //userClaimsFinal.Add(new Claim(ProfileServiceManager.Constants.ClaimKey, Constants.ArbitraryIdentityProfileService));
            if (originAuthTimeClaim != null)
            {
                userClaimsFinal.Add(originAuthTimeClaim);
            }
            context.Result = new GrantValidationResult(principal.GetSubjectId(), ArbitraryIdentityExtensionGrant.Constants.ArbitraryIdentity, userClaimsFinal);
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
        public string GrantType => Constants.ArbitraryIdentity;
    }
}
