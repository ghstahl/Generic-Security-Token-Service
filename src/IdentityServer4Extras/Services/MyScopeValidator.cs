using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace IdentityServer4Extras.Services
{
    public class MyScopeValidator : ScopeValidator
    {
        public MyScopeValidator(IResourceStore store, ILogger<MyScopeValidator> logger) : base(store, logger)
        {
        }

        public override async Task<bool> AreScopesAllowedAsync(Client client, IEnumerable<string> requestedScopes)
        {
            return true;
        }

        public override async Task<bool> AreScopesValidAsync(IEnumerable<string> requestedScopes,
            bool filterIdentityScopes = false)
        {
            if (requestedScopes.Contains(IdentityServerConstants.StandardScopes.OfflineAccess))
            {
                GrantedResources.OfflineAccess = true;
                requestedScopes = requestedScopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess).ToArray();
            }
            return true;
        }
    }
}