using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Validators
{
    public class PluginHostClientSecretValidator : IClientSecretValidator
    {
        private ILogger<PluginHostClientSecretValidator> _logger;
        private List<IClientSecretValidatorPlugin> _plugins;
        private readonly IClientStore _clients;
        private readonly SecretParser _parser;
        private IEventService _events;

        public PluginHostClientSecretValidator(
            IEnumerable<IClientSecretValidatorPlugin> plugins,
            IClientStore clients,
            SecretParser parser,
            IEventService events,
            ILogger<PluginHostClientSecretValidator> logger)
        {
            _plugins = plugins.ToList();
            _clients = clients;
            _parser = parser;
            _events = events;
            _logger = logger;
        }
        public async Task<ClientSecretValidationResult> ValidateAsync(HttpContext context)
        {
            var fail = new ClientSecretValidationResult()
            {
                IsError = true,
                Error = OidcConstants.TokenErrors.InvalidClient,
            };
            if (!_plugins.Any())
            {
                await RaiseFailureEventAsync("unknown", "No client id found");
                fail.ErrorDescription = "No IClientSecretValidatorPlugin were found!";
                _logger.LogError(fail.ErrorDescription);
                return fail;
            }
            var parsedSecret = await _parser.ParseAsync(context);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No client id found");
                fail.ErrorDescription = "No client identifier found";
                _logger.LogError(fail.ErrorDescription);
                return fail;
            }

            // load client
            var client = await _clients.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (client == null)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown client");
                fail.ErrorDescription = $"No client with id '{parsedSecret.Id}' found. aborting";
                _logger.LogError(fail.ErrorDescription);
                return fail;
            }
            foreach (var plugin in _plugins)
            {
                var result = await plugin.ValidateAsync(context);
                if (result.IsError)
                {
                    return result;
                }
            }

            var success = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client,
                Secret = parsedSecret
            };
            return success;
        }
        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ClientAuthenticationFailureEvent(clientId, message));
        }
    }
}