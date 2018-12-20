using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Events;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Models;
using Microsoft.Extensions.Logging;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Http;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class ArbitraryResourceOwnerExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryResourceOwnerExtensionGrantValidator> _logger;
        private readonly IdentityServerOptions _options;
        private ArbitraryResourceOwnerRequestValidator _arbitraryResourceOwnerRequestValidator;
        private PrincipalAugmenter _principalAugmenter;
        private IEventService _events;
        private IClientSecretValidator _clientValidator;
        private IHttpContextAccessor _httpContextAccessor;

        public IdentityServerRequestRecord IdentityServerRequestRecord { get; }

        public ArbitraryResourceOwnerExtensionGrantValidator(
            IdentityServerOptions options,
            IClientSecretValidator clientValidator,
            ILogger<ArbitraryResourceOwnerExtensionGrantValidator> logger,
            ArbitraryResourceOwnerRequestValidator arbitraryResourceOwnerRequestValidator,
            PrincipalAugmenter principalAugmenter,
            IEventService events,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _options = options;
            _clientValidator = clientValidator;
            _arbitraryResourceOwnerRequestValidator = arbitraryResourceOwnerRequestValidator;
            _principalAugmenter = principalAugmenter;
            _events = events;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");
           
            string grantType = null;
            string clientId = "";
            try
            {
                if (context == null) throw new ArgumentNullException(nameof(context));
                var raw = context.Request.Raw;
                var validatedRequest = new ValidatedTokenRequest
                {
                    Raw = raw ?? throw new ArgumentNullException(nameof(raw)),
                    Options = _options
                };
                // validate HTTP for clients
                if (HttpMethods.IsPost(_httpContextAccessor.HttpContext.Request.Method) && _httpContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    // validate client
                    var clientResult = await _clientValidator.ValidateAsync(_httpContextAccessor.HttpContext);
                    if (!clientResult.IsError)
                    {
                        validatedRequest.SetClient(clientResult.Client);
                        clientId = clientResult.Client.ClientId;
                    }
                }
                /////////////////////////////////////////////
                // get grant type.  This has already been validated by the time it gets here.
                /////////////////////////////////////////////
                grantType = validatedRequest.Raw.Get(OidcConstants.TokenRequest.GrantType);
                validatedRequest.GrantType = grantType;

                var customTokenRequestValidationContext = new CustomTokenRequestValidationContext()
                {
                    Result = new TokenRequestValidationResult(validatedRequest)
                };
                await _arbitraryResourceOwnerRequestValidator.ValidateAsync(customTokenRequestValidationContext);
                if (customTokenRequestValidationContext.Result.IsError)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                        customTokenRequestValidationContext.Result.Error);
                    throw new Exception("Invalid Request");
                }

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
                        throw new Exception(errorDescription);
                    }
                }

                context.Result = new GrantValidationResult(principal.GetSubjectId(),
                    ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner);
            }
            catch (Exception e)
            {
            }

            if (context.Result.IsError)
            {
                await _events.RaiseAsync(new ExtensionGrantValidationFailureEvent(
                    clientId,
                    grantType,
                    context.Result.Error));
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
        }

        public string GrantType => Constants.ArbitraryResourceOwner;
    }
}
