using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityServer4Extras.Extensions;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Validators
{
    /// <summary>
    /// The token revocation request validator
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.ITokenRevocationRequestValidator" />
    public class SubjectTokenRevocationRequestValidator : ITokenRevocationRequestValidator
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.Validation.TokenRevocationRequestValidator.TokenRevocationRequestValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SubjectTokenRevocationRequestValidator(ILogger<SubjectTokenRevocationRequestValidator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// parameters
        /// or
        /// client
        /// </exception>
        public Task<TokenRevocationRequestValidationResult> ValidateRequestAsync(NameValueCollection parameters, Client client)
        {
            _logger.LogTrace("ValidateRequestAsync called");

            if (parameters == null)
            {
                _logger.LogError("no parameters passed");
                throw new ArgumentNullException(nameof(parameters));
            }

            if (client == null)
            {
                _logger.LogError("no client passed");
                throw new ArgumentNullException(nameof(client));
            }

            ////////////////////////////
            // make sure token is present
            ///////////////////////////
            var token = parameters.Get("token");
            if (token.IsMissing())
            {
                _logger.LogError("No token found in request");
                return Task.FromResult(new TokenRevocationRequestValidationResult
                {
                    IsError = true,
                    Error = OidcConstants.TokenErrors.InvalidRequest
                });
            }

            var result = new TokenRevocationRequestValidationResult
            {
                IsError = false,
                Token = token,
                Client = client
            };

            ////////////////////////////
            // check token type hint
            ///////////////////////////
            var hint = parameters.Get("token_type_hint");
            if (hint.IsPresent())
            {
                if (IdentityServer4Extras.Constants.SupportedTokenTypeHints.Contains(hint))
                {
                    _logger.LogDebug("Token type hint found in request: {tokenTypeHint}", hint);
                    result.TokenTypeHint = hint;
                }
                else
                {
                    _logger.LogError("Invalid token type hint: {tokenTypeHint}", hint);
                    result.IsError = true;
                    result.Error = IdentityServer4Extras.Constants.RevocationErrors.UnsupportedTokenType;
                }
            }

            _logger.LogDebug("ValidateRequestAsync result: {validateRequestResult}", result);

            return Task.FromResult(result);
        }
    }
}