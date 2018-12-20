using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArbitraryNoSubjectExtensionGrant
{
    public class ArbitraryNoSubjectExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ILogger<ArbitraryNoSubjectExtensionGrantValidator> _logger;
        private readonly IdentityServerOptions _options; 
        private ArbitraryNoSubjectRequestValidator _arbitraryNoSubjectRequestValidator; 
        private IClientSecretValidator _clientValidator;
        private IHttpContextAccessor _httpContextAccessor;

        public ArbitraryNoSubjectExtensionGrantValidator( 
            IClientSecretValidator clientValidator,
            IdentityServerOptions options,
            ILogger<ArbitraryNoSubjectExtensionGrantValidator> logger,
            ArbitraryNoSubjectRequestValidator arbitraryNoSubjectRequestValidator,
            IHttpContextAccessor httpContextAccessor)
        { 
            _clientValidator = clientValidator;
            _logger = logger;
            _options = options;
            _arbitraryNoSubjectRequestValidator = arbitraryNoSubjectRequestValidator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogDebug("Start token request validation");

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
            await _arbitraryNoSubjectRequestValidator.ValidateAsync(customTokenRequestValidationContext);
            if (customTokenRequestValidationContext.Result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
                    customTokenRequestValidationContext.Result.Error);
                return;
            }

            // validate HTTP for clients
            if (HttpMethods.IsPost(_httpContextAccessor.HttpContext.Request.Method) && _httpContextAccessor.HttpContext.Request.HasFormContentType)
            {
                // validate client
                var clientResult = await _clientValidator.ValidateAsync(_httpContextAccessor.HttpContext);
                if (!clientResult.IsError)
                {
                    validatedRequest.SetClient(clientResult.Client);
                }
            }

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
            context.Result = new GrantValidationResult();
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
        public string GrantType => Constants.ArbitraryNoSubject;
    }
}
