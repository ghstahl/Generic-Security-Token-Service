using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Validators
{
    /// <summary>
    /// Validates a client secret using the registered secret validators and parsers
    /// </summary>
    public class NoSecretRefreshClientSecretValidator : IClientSecretValidatorExtra, IClientSecretValidator
    {
        private IClientSecretValidator StockClientSecretValidator { get; set; }
        private readonly ILogger _logger;
        private readonly IClientStore _clients;
        private readonly IEventService _events;
        private readonly SecretValidator _validator;
        private readonly SecretParser _parser;
        private readonly ISecretParserExtra _secretParserExtra;
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.Validation.ClientSecretValidator"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public NoSecretRefreshClientSecretValidator(IClientStore clients, SecretParser parser,
            ISecretParserExtra secretParserExtra,
            SecretValidator validator, 
            IEventService events, ILogger<IdentityServer4.Validation.ClientSecretValidator> logger)
        {
            StockClientSecretValidator = new ClientSecretValidator(clients,parser,validator,events,logger);
            _clients = clients;
            _parser = parser;
            _secretParserExtra = secretParserExtra;
            _validator = validator;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Validates the current request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<ClientSecretValidationResult> ValidateAsync(HttpContext context)
        {
            _logger.LogDebug("Start client validation");

            var fail = new ClientSecretValidationResult
            {
                IsError = true
            };

            var parsedSecret = await _parser.ParseAsync(context);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No client id found");

                _logger.LogError("No client identifier found");
                return fail;
            }

            // load client
            var client = await _clients.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (client == null)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown client");

                _logger.LogError("No client with id '{clientId}' found. aborting", parsedSecret.Id);
                return fail;
            }
            var form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
            var grantType = form.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType == OidcConstants.GrantTypes.RefreshToken)
            {
                // upcast;
                ClientExtra clientExtra = client as ClientExtra;
                if (!clientExtra.RequireRefreshClientSecret)
                {
                    _logger.LogDebug("Public Client - skipping secret validation success");
                    _logger.LogDebug("Client validation success");

                    var success = new ClientSecretValidationResult
                    {
                        IsError = false,
                        Client = client,
                        Secret = parsedSecret
                    };

                    await RaiseSuccessEventAsync(client.ClientId, parsedSecret.Type);
                    return success;
                }
            }
            return await StockClientSecretValidator.ValidateAsync(context);
        }

        public async Task<ClientSecretValidationResult> ValidateAsync(IFormCollection formCollection)
        {
            _logger.LogDebug("Start client validation");

            var fail = new ClientSecretValidationResult
            {
                IsError = true
            };

            var parsedSecret = await _secretParserExtra.ParseAsync(formCollection);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No client id found");

                _logger.LogError("No client identifier found");
                return fail;
            }
            // load client
            var client = await _clients.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (client == null)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown client");

                _logger.LogError("No client with id '{clientId}' found. aborting", parsedSecret.Id);
                return fail;
            }
            var form =  formCollection.AsNameValueCollection();
            var grantType = form.Get(OidcConstants.TokenRequest.GrantType);
            if (grantType == OidcConstants.GrantTypes.RefreshToken)
            {
                // upcast;
                ClientExtra clientExtra = client as ClientExtra;
                if (!clientExtra.RequireRefreshClientSecret)
                {
                    _logger.LogDebug("Public Client - skipping secret validation success");
                    _logger.LogDebug("Client validation success");

                    var successRefreshToken = new ClientSecretValidationResult
                    {
                        IsError = false,
                        Client = client,
                        Secret = parsedSecret
                    };

                    await RaiseSuccessEventAsync(client.ClientId, parsedSecret.Type);
                    return successRefreshToken;
                }
            }
            if (!client.RequireClientSecret || client.IsImplicitOnly())
            {
                _logger.LogDebug("Public Client - skipping secret validation success");
            }
            else
            {
                var result = await _validator.ValidateAsync(parsedSecret, client.ClientSecrets);
                if (result.Success == false)
                {
                    await RaiseFailureEventAsync(client.ClientId, "Invalid client secret");
                    _logger.LogError("Client secret validation failed for client: {clientId}.", client.ClientId);

                    return fail;
                }
            }

            _logger.LogDebug("Client validation success");

            var success = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client,
                Secret = parsedSecret
            };

            await RaiseSuccessEventAsync(client.ClientId, parsedSecret.Type);
            return success;
        }

        private Task RaiseSuccessEventAsync(string clientId, string authMethod)
        {
            return _events.RaiseAsync(new ClientAuthenticationSuccessEvent(clientId, authMethod));
        }

        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ClientAuthenticationFailureEvent(clientId, message));
        }
    }
}
