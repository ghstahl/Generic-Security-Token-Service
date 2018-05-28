using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Endpoints
{
    /// <summary>
    /// The token endpoint
    /// </summary>
    /// <seealso cref="IEndpointHandlerExtra" />
    public class TokenEndpointExtra : IEndpointHandlerExtra
    {
        private readonly ITokenRequestValidator _requestValidator;
        private readonly ITokenResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger _logger;
        private readonly IClientStore _clients;
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenEndpointExtra" /> class.
        /// </summary>
        /// <param name="clients">The clients store.</param>
        /// <param name="requestValidator">The request validator.</param>
        /// <param name="responseGenerator">The response generator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public TokenEndpointExtra(
            IClientStore clients,
            ITokenRequestValidator requestValidator,
            ITokenResponseGenerator responseGenerator,
            IEventService events,
            ILogger<TokenEndpointExtra> logger)
        {
            _clients = clients;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
            _logger = logger;
        }
        public async Task<IEndpointResult> ProcessAsync(IFormCollection formCollection)
        {
            var result = await ProcessRawAsync(formCollection);
            if (result.TokenErrorResult != null)
                return result.TokenErrorResult;
            return result.TokenResult;
        }

        public async Task<TokenRawResult> ProcessRawAsync(IFormCollection formCollection)
        {
            _logger.LogTrace("Processing token request.");

            var rawResult = new TokenRawResult();
            // validate HTTP
            if (formCollection.IsNullOrEmpty())
            {
                _logger.LogWarning($"Invalid {nameof(formCollection)} for token endpoint");
                rawResult.TokenErrorResult = Error(OidcConstants.TokenErrors.InvalidRequest);
                return rawResult;
            }

            var clientId = formCollection[OidcConstants.TokenRequest.ClientId];
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
            if (client == null)
            {
                _logger.LogError($"No client with id '{clientId}' found. aborting");
                rawResult.TokenErrorResult = Error($"{OidcConstants.TokenRequest.ClientId} bad",
                    $"No client with id '{clientId}' found. aborting");
                return rawResult;
            }
            var clientSecretValidationResult = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client,
                Secret = null
            };

            // validate request
            var form = formCollection.AsNameValueCollection();
            _logger.LogTrace("Calling into token request validator: {type}", _requestValidator.GetType().FullName);
            var requestResult = await _requestValidator.ValidateRequestAsync(form, clientSecretValidationResult);

            if (requestResult.IsError)
            {
                await _events.RaiseAsync(new TokenIssuedFailureEvent(requestResult));
                rawResult.TokenErrorResult = Error(requestResult.Error, requestResult.ErrorDescription, requestResult.CustomResponse);
                return rawResult;
            }

            // create response
            _logger.LogTrace("Calling into token request response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(requestResult);

            await _events.RaiseAsync(new TokenIssuedSuccessEvent(response, requestResult));
            LogTokens(response, requestResult);

            // return result
            _logger.LogDebug("Token request success.");
            rawResult.TokenResult = new TokenResult(response);
            return rawResult;
        }

        private TokenErrorResult Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            var response = new TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };

            return new TokenErrorResult(response);
        }

        private void LogTokens(TokenResponse response, TokenRequestValidationResult requestResult)
        {
            var clientId = $"{requestResult.ValidatedRequest.Client.ClientId} ({requestResult.ValidatedRequest.Client?.ClientName ?? "no name set"})";
            var subjectId = requestResult.ValidatedRequest.Subject?.GetSubjectId() ?? "no subject";

            if (response.IdentityToken != null)
            {
                _logger.LogTrace("Identity token issued for {clientId} / {subjectId}: {token}", clientId, subjectId, response.IdentityToken);
            }
            if (response.RefreshToken != null)
            {
                _logger.LogTrace("Refresh token issued for {clientId} / {subjectId}: {token}", clientId, subjectId, response.RefreshToken);
            }
            if (response.AccessToken != null)
            {
                _logger.LogTrace("Access token issued for {clientId} / {subjectId}: {token}", clientId, subjectId, response.AccessToken);
            }
        }
    }
}
