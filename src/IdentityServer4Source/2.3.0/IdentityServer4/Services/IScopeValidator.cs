using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4.Services
{
    public interface IScopeValidator
    {
        Resources GrantedResources { get; }
        Resources RequestedResources { get; }
        bool ContainsOpenIdScopes { get; }
        bool ContainsApiResourceScopes { get; }
        bool ContainsOfflineAccessScope { get; }
        Task<bool> AreScopesAllowedAsync(Client client, IEnumerable<string> requestedScopes);
        Task<bool> AreScopesValidAsync(IEnumerable<string> requestedScopes,
            bool filterIdentityScopes = false);

        bool ValidateRequiredScopes(IEnumerable<string> consentedScopes);
        void SetConsentedScopes(IEnumerable<string> consentedScopes);
        bool IsResponseTypeValid(string responseType);
    }
}
