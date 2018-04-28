using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer4Extras
{
    public class ClientRefreshTokenRequiredSecretValidator : ISecretValidator
    {
        public async Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            if (parsedSecret.Type == "client_refresh_token_required_secret_check"
                && string.Compare((string)parsedSecret.Credential, "success",StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return new SecretValidationResult()
                {
                    IsError = false,
                    Success = true
                };
            }
            return null;
        }
    }
}