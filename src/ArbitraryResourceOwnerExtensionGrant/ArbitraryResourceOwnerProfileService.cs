using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Newtonsoft.Json;
using ProfileServiceManager;

namespace ArbitraryResourceOwnerExtensionGrant
{
    public class ArbitraryResourceOwnerProfileService : IProfileService, IProfileServicePlugin
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Subject != null)
            {
                var query = from item in context.Subject.Claims
                    where item.Type == "amr"
                    select item.Value;
                var grantType = query.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(grantType) &&
                    string.Compare(grantType, ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryResourceOwner, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var queryClaims = from item in context.Subject.Claims
                        where item.Type == ArbitraryResourceOwnerExtensionGrant.Constants.ArbitraryClaims
                        select item.Value;
                    var claimsJson = queryClaims.FirstOrDefault();
                    if (claimsJson != null)
                    {
                        var values =
                            JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(claimsJson);
                        // paranoia check.  In no way can we allow creation which tries to spoof someone elses client_id.
                        var trimmedQuery = from item in values
                            where String.Compare(item.Key, "client_id", StringComparison.OrdinalIgnoreCase) != 0
                            select item;
                        var finalClaims = (
                            from item in trimmedQuery
                            from c in item.Value
                            select new Claim(item.Key, c)).ToList();

                        context.IssuedClaims.AddRange(finalClaims);
                        context.IssuedClaims.Add(new Claim(ProfileServiceManager.Constants.ClaimKey, Constants.ArbitraryResourceOwnerProfileService));
                    }
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
          
        }

        public string Name => Constants.ArbitraryResourceOwnerProfileService;
        public IProfileService ProfileService => this;
    }
}