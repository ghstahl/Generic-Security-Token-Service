using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServerRequestTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using IdentityServerRequestTracker.Services;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net;
using IdentityServer4.Models;
using IdentityServer4Extras.Extensions;

namespace IdentityServer4Extras.Endpoints
{
    /// <summary>
    /// The token endpoint
    /// </summary>
    /// <seealso cref="ITokenEndpointHandlerExtra" />
    public class TokenTokenEndpointExtra : ITokenEndpointHandlerExtra
    {
        private readonly ITokenRequestValidator _requestValidator;
        private readonly ITokenResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger _logger;
        private readonly IClientStore _clients;
        private IScopedStorage _scopedStorage;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenRevocationRequestValidator _revocationRequestValidator;
        private ITokenRevocationResponseGenerator _revocationResponseGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenTokenEndpointExtra" /> class.
        /// </summary>
        /// <param name="clients">The clients store.</param>
        /// <param name="requestValidator">The request validator.</param>
        /// <param name="responseGenerator">The response generator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public TokenTokenEndpointExtra(
            IHttpContextAccessor httpContextAccessor,
            IClientStore clients,
            IScopedStorage scopedStorage,
            ITokenRequestValidator requestValidator,
            ITokenResponseGenerator responseGenerator,
            ITokenRevocationRequestValidator revocationRequestValidator,
            ITokenRevocationResponseGenerator revocationResponseGenerator,
            IEventService events,
            ILogger<TokenTokenEndpointExtra> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clients = clients;
            _scopedStorage = scopedStorage;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _revocationRequestValidator = revocationRequestValidator;
            _revocationResponseGenerator = revocationResponseGenerator;
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
            var requestRecord = new IdentityServerRequestRecord
            {
                HttpContext = _httpContextAccessor.HttpContext,
                EndpointKey = "extra",
                Client = client
            };
            _scopedStorage.Storage["IdentityServerRequestRecord"] = requestRecord;
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

        public async Task<IEndpointResult> ProcessAsync(ArbitraryResourceOwnerRequest request)
        {
            var result = await ProcessRawAsync(request);
            if (result.TokenErrorResult != null)
                return result.TokenErrorResult;
            return result.TokenResult;
        }

        public async Task<IEndpointResult> ProcessAsync(ArbitraryNoSubjectRequest request)
        {
            var result = await ProcessRawAsync(request);
            if (result.TokenErrorResult != null)
                return result.TokenErrorResult;
            return result.TokenResult;
        }

        public async Task<IEndpointResult> ProcessAsync(ArbitraryIdentityRequest request)
        {
            var result = await ProcessRawAsync(request);
            if (result.TokenErrorResult != null)
                return result.TokenErrorResult;
            return result.TokenResult;
        }

        public async Task<IEndpointResult> ProcessAsync(RefreshTokenRequest request)
        {
            var result = await ProcessRawAsync(request);
            if (result.TokenErrorResult != null)
                return result.TokenErrorResult;
            return result.TokenResult;
        }

        public async Task<IEndpointResult> ProcessAsync(RevocationRequest request)
        {
            var result = await ProcessRawAsync(request);
            if (result.ErrorResult != null)
                return result.ErrorResult;
            return result.StatusCodeResult;
        }

        public async Task<TokenRawResult> ProcessRawAsync(ArbitraryResourceOwnerRequest request)
        {
            Dictionary<string, StringValues> fields = new Dictionary<string, StringValues>
            {
                {"client_id", request.ClientId},
                {"grant_type", "arbitrary_resource_owner"},
                {"subject", request.Subject}
            };
            string scope = "";
            if (request.Scopes != null)
            {
                foreach (var item in request.Scopes)
                {
                    scope += $"{item} ";
                }
                scope.TrimEnd();
                fields.Add("scope", scope);
            }

            if (request.ArbitraryClaims != null)
            {
                fields.Add("arbitrary_claims", JsonConvert.SerializeObject(request.ArbitraryClaims));
            }
            if (request.ArbitraryAmrs != null)
            {
                fields.Add("arbitrary_amrs", JsonConvert.SerializeObject(request.ArbitraryAmrs));
            }
            if (request.ArbitraryAudiences != null)
            {
                fields.Add("arbitrary_audiences", JsonConvert.SerializeObject(request.ArbitraryAudiences));
            }
            if (request.CustomPayload != null)
            {
                fields.Add("custom_payload", JsonConvert.SerializeObject(request.CustomPayload));
            }
            var formCollection = new FormCollection(fields);
            return await ProcessRawAsync(formCollection);
        }

        public async Task<TokenRawResult> ProcessRawAsync(ArbitraryNoSubjectRequest request)
        {
            Dictionary<string, StringValues> fields = new Dictionary<string, StringValues>
            {
                {"client_id", request.ClientId},
                {"grant_type", "arbitrary_no_subject"},
            };
            string scope = "";
            if (request.Scopes != null)
            {
                foreach (var item in request.Scopes)
                {
                    scope += $"{item} ";
                }
                scope.TrimEnd();
                fields.Add("scope", scope);
            }

            if (request.ArbitraryClaims != null)
            {
                fields.Add("arbitrary_claims", JsonConvert.SerializeObject(request.ArbitraryClaims));
            }
            if (request.ArbitraryAmrs != null)
            {
                fields.Add("arbitrary_amrs", JsonConvert.SerializeObject(request.ArbitraryAmrs));
            }
            if (request.ArbitraryAudiences != null)
            {
                fields.Add("arbitrary_audiences", JsonConvert.SerializeObject(request.ArbitraryAudiences));
            }
            if (request.CustomPayload != null)
            {
                fields.Add("custom_payload", JsonConvert.SerializeObject(request.CustomPayload));
            }

            var formCollection = new FormCollection(fields);
            return await ProcessRawAsync(formCollection);
        }

        public async Task<TokenRawResult> ProcessRawAsync(ArbitraryIdentityRequest request)
        {
            Dictionary<string, StringValues> fields = new Dictionary<string, StringValues>
            {
                {"client_id", request.ClientId},
                {"grant_type", "arbitrary_identity"},
                {"subject", request.Subject}
            };
            string scope = "";
            if (request.Scopes != null)
            {
                foreach (var item in request.Scopes)
                {
                    scope += $"{item} ";
                }
                scope.TrimEnd();
                fields.Add("scope", scope);
            }

            if (request.ArbitraryClaims != null)
            {
                fields.Add("arbitrary_claims", JsonConvert.SerializeObject(request.ArbitraryClaims));
            }
            if (request.ArbitraryAmrs != null)
            {
                fields.Add("arbitrary_amrs", JsonConvert.SerializeObject(request.ArbitraryAmrs));
            }
            if (request.ArbitraryAudiences != null)
            {
                fields.Add("arbitrary_audiences", JsonConvert.SerializeObject(request.ArbitraryAudiences));
            }
            if (request.CustomPayload != null)
            {
                fields.Add("custom_payload", JsonConvert.SerializeObject(request.CustomPayload));
            }

            var formCollection = new FormCollection(fields);
            return await ProcessRawAsync(formCollection);
        }

        public async Task<TokenRawResult> ProcessRawAsync(RefreshTokenRequest request)
        {
            Dictionary<string, StringValues> fields = new Dictionary<string, StringValues>
            {
                {"client_id", request.ClientId},
                {"grant_type", "refresh_token"},
                {"refresh_token", request.RefreshToken}
            };
            var formCollection = new FormCollection(fields);
            return await ProcessRawAsync(formCollection);
        }

        public async Task<RevocationRawResult> ProcessRawAsync(RevocationRequest request)
        {
            Dictionary<string, StringValues> fields = new Dictionary<string, StringValues>
            {
                {"client_id", request.ClientId},
                {"token_type_hint", request.TokenTypHint},
                {"token", request.Token},
                {"revoke_all_subjects", request.RevokeAllSubjects}
            };
            var formCollection = new FormCollection(fields);
            var result =  await ProcessRawRevocationAsync(formCollection);
            return result;
           
        }
        public async Task<RevocationRawResult> ProcessRawRevocationAsync(IFormCollection formCollection)
        {
 
            _logger.LogTrace("Processing token request.");

            var rawResult = new RevocationRawResult()
            {
                StatusCodeResult = new StatusCodeResult(HttpStatusCode.BadRequest)
            };
          
            // validate HTTP
            if (formCollection.IsNullOrEmpty())
            {
                _logger.LogWarning($"Invalid {nameof(formCollection)} for token endpoint");
                rawResult.ErrorResult = ErrorRevocation(OidcConstants.TokenErrors.InvalidRequest);
                return rawResult;
            }

            var clientId = formCollection[OidcConstants.TokenRequest.ClientId];
            var client = await _clients.FindEnabledClientByIdAsync(clientId);
            if (client == null)
            {
                _logger.LogError($"No client with id '{clientId}' found. aborting");
                rawResult.ErrorResult = ErrorRevocation($"{OidcConstants.TokenRequest.ClientId} bad");
                return rawResult;
            }

            var clientValidationResult = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client,
                Secret = null
            };

            // validate request
            var form = formCollection.AsNameValueCollection();
            _logger.LogTrace("Calling into token revocation request validator: {type}", _requestValidator.GetType().FullName);
            var requestValidationResult = await _revocationRequestValidator.ValidateRequestAsync(form, clientValidationResult.Client);
            if (requestValidationResult.IsError)
            {
                rawResult.ErrorResult = new TokenRevocationErrorResult(requestValidationResult.Error);
                return rawResult;
            }

            _logger.LogTrace("Calling into token revocation response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _revocationResponseGenerator.ProcessAsync(requestValidationResult);

            if (response.Success)
            {
                _logger.LogInformation("Token successfully revoked");
                await _events.RaiseAsync(new TokenRevokedSuccessEvent(requestValidationResult, requestValidationResult.Client));
            }
            else
            {
                _logger.LogInformation("No matching token found");
            }

            if (response.Error.IsPresent())
            {
                rawResult.ErrorResult = new TokenRevocationErrorResult(response.Error);
                return rawResult;
            }

            rawResult.StatusCodeResult = new StatusCodeResult(HttpStatusCode.OK);
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
        private TokenRevocationErrorResult ErrorRevocation(string error)
        {
            return new TokenRevocationErrorResult(error);
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
