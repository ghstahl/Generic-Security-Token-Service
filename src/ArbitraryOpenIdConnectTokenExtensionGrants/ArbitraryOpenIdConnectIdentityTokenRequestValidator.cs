using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace ArbitraryOpenIdConnectTokenExtensionGrants
{
    public class ArbitraryOpenIdConnectIdentityTokenRequestValidator
    {
        private readonly ILogger<ArbitraryOpenIdConnectIdentityTokenRequestValidator> _logger;

        private static List<string> _requiredArguments;
        private static List<string> RequiredArguments => _requiredArguments ??
                                                                  (_requiredArguments =
                                                                      new List<string>
                                                                      {
                                                                          "subject",
                                                                          "client_id",
                                                                          "client_secret",
                                                                          "id_token",
                                                                          "authority",
                                                                          "arbitrary_claims"
                                                                      });
        public ArbitraryOpenIdConnectIdentityTokenRequestValidator(
            ILogger<ArbitraryOpenIdConnectIdentityTokenRequestValidator> logger)
        {
            _logger = logger;
        }
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var raw = context.Result.ValidatedRequest.Raw;
            var rr = raw.AllKeys.ToDictionary(k => k, k => raw[(string)k]);
            var error = false;
            var los = new List<string>();

            var result = RequiredArguments.Except(rr.Keys);
            if (result.Any())
            {
                error = true;
                los.AddRange(result.Select(item => $"{item} is missing!"));

            }
            if (error)
            {
                context.Result.IsError = true;
                context.Result.Error = String.Join<string>(" | ", los); ;
            }
        }
    }
}