using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Endpoints
{
    internal class Authorize2Endpoint : AuthorizeEndpointBase
    {
        private readonly BearerTokenUsageValidator _tokenUsageValidator;
        private readonly IUserInfoRequestValidator _requestValidator;
        private readonly IUserInfoResponseGenerator _responseGenerator;
        private readonly ILogger _logger;

        private IAuthorize2RequestValidator _validator2;
        private readonly IClientSecretValidator _clientValidator;
       
        public Authorize2Endpoint(
            BearerTokenUsageValidator tokenUsageValidator,
            IUserInfoRequestValidator requestValidator,
            IUserInfoResponseGenerator responseGenerator,
            IEventService events,
            IClientSecretValidator clientValidator,
            ILogger<AuthorizeEndpoint> logger,
            IAuthorize2RequestValidator validator2,
            IAuthorizeRequestValidator validator,
            IAuthorizeInteractionResponseGenerator interactionGenerator,
            IAuthorizeResponseGenerator authorizeResponseGenerator,
            IUserSession userSession )
            : base(events, logger, validator, interactionGenerator, authorizeResponseGenerator, userSession)
        {
            _validator2 = validator2;
            _clientValidator = clientValidator; 
            _tokenUsageValidator = tokenUsageValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
        }

        public override async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            Logger.LogDebug("Start authorize2 request");

            if (!HttpMethods.IsPost(context.Request.Method))
            {
                Logger.LogWarning("Invalid HTTP request for token endpoint");
                return Error(Authorize2Constants.Authorize2Errors.InvalidRequest);
            }

            var tokenUsageResult = await _tokenUsageValidator.ValidateAsync(context);
            if (tokenUsageResult.TokenFound == false)
            {
                var error = "No access token found.";

                _logger.LogError(error);
                return Error(OidcConstants.ProtectedResourceErrors.InvalidToken);
            }

            if (!context.Request.HasFormContentType)
            {
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
            }

            // validate the request
            Logger.LogTrace("Calling into userinfo request validator: {type}", _requestValidator.GetType().FullName);
            var validationResult = await _requestValidator.ValidateRequestAsync(tokenUsageResult.Token);

            if (validationResult.IsError)
            {
                //_logger.LogError("Error validating  validationResult.Error);
                return Error(validationResult.Error);
            }


            // validate client
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult.Client == null)
            {
                return Error(Authorize2Constants.Authorize2Errors.InvalidClient);
            }

            // validate request
            NameValueCollection values = (await context.Request.ReadFormAsync()).AsNameValueCollection();
            Logger.LogTrace("Calling into token request validator: {type}", _validator2.GetType().FullName);
            var requestResult = await _validator2.ValidateRequestAsync(values, clientResult);

            // var user = await UserSession.GetUserAsync();
            var user = validationResult.Subject;
            var result = await ProcessAuthorizeRequestAsync(values, user, null);

            Logger.LogTrace("End authorize request. result type: {0}", result?.GetType().ToString() ?? "-none-");

        //    return Error(Authorize2Constants.Authorize2Errors.InvalidClient);
            return new Authorize2Result(result as AuthorizeResult);
        }
        private Authorize2ErrorResult Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            var response = new Authorize2ErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };

            return new Authorize2ErrorResult(response);
        }
    }
}