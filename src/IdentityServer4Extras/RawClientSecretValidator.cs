using System.Collections.Specialized;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Extensions;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras
{
    public class RawClientSecretValidator : IRawClientSecretValidator
    {
        private readonly ILogger _logger;
        private readonly IClientStore _clients;
        private readonly IEventService _events;
        private readonly SecretValidator _validator;
        private readonly SecretParser _parser;
        private readonly IdentityServerOptions _options;
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSecretValidator"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="options">The options.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public RawClientSecretValidator(IClientStore clients, IdentityServerOptions options,
            SecretValidator validator, IEventService events, ILogger<ClientSecretValidator> logger)
        {
            _clients = clients;
            _options = options;
            _validator = validator;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        /// Validates the current request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<ClientSecretValidationResult> ValidateAsync(NameValueCollection nvc)
        {
            _logger.LogDebug("Start client validation");

            var fail = new ClientSecretValidationResult
            {
                IsError = true
            };

            var id = nvc.Get("client_id");
            var secret = nvc.Get("client_secret");
            ParsedSecret parsedSecret = null;
            // client id must be present
            if (id.IsPresent())
            {
                if (id.Length > _options.InputLengthRestrictions.ClientId)
                {
                    _logger.LogError("Client ID exceeds maximum length.");
                    return null;
                }

                if (secret.IsPresent())
                {
                    if (secret.Length > _options.InputLengthRestrictions.ClientSecret)
                    {
                        _logger.LogError("Client secret exceeds maximum length.");
                        return null;
                    }

                    parsedSecret = new ParsedSecret
                    {
                        Id = id,
                        Credential = secret,
                        Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
                    };
                }
            }

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

            if (!client.RequireClientSecret)
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