using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;

namespace IdentityServer4Extras.Services
{
    public class MyScopeValidator :  IScopeValidator
    {
        private ScopeValidator _scopeValidator;

        public MyScopeValidator(
            IResourceStore store,
            ScopeValidator scopeValidator,ILogger<MyScopeValidator> logger)  
        {
            _scopeValidator = scopeValidator;
        }

        public Resources GrantedResources => _scopeValidator.GrantedResources;
        public Resources RequestedResources => _scopeValidator.RequestedResources;
        public bool ContainsOpenIdScopes => _scopeValidator.ContainsOpenIdScopes;
        public bool ContainsApiResourceScopes => _scopeValidator.ContainsApiResourceScopes;
        public bool ContainsOfflineAccessScope => _scopeValidator.ContainsOfflineAccessScope;

        public async Task<bool> AreScopesAllowedAsync(Client client, IEnumerable<string> requestedScopes)
        {
            return true;
        }

        public  async Task<bool> AreScopesValidAsync(IEnumerable<string> requestedScopes,
            bool filterIdentityScopes = false)
        {
            if (requestedScopes.Contains(IdentityServerConstants.StandardScopes.OfflineAccess))
            {
                GrantedResources.OfflineAccess = true;
                requestedScopes = requestedScopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess).ToArray();
            }
            var distinct = requestedScopes.DistinctBy(i => i);
            var grantedApis = from item in requestedScopes
                let c = new ApiResource(item)
                select c;
            foreach (var grantedItem in grantedApis)
            {
                GrantedResources.ApiResources.Add(grantedItem);
            }
            return true;
        }

        public bool ValidateRequiredScopes(IEnumerable<string> consentedScopes)
        {
            return _scopeValidator.ValidateRequiredScopes(consentedScopes);
        }

        public void SetConsentedScopes(IEnumerable<string> consentedScopes)
        {
            _scopeValidator.SetConsentedScopes(consentedScopes);
        }

        public bool IsResponseTypeValid(string responseType)
        {
            return _scopeValidator.IsResponseTypeValid(responseType);
        }
    }
}