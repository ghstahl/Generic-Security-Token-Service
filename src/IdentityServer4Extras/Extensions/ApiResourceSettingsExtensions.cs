using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4Extras.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4Extras.Extensions
{
    public static class ApiResourceSettingsExtensions
    {
        public static List<ApiResource> LoadApiResourcesFromSettings(this IConfiguration configuration)
        {

            IConfigurationSection section = configuration.GetSection("apiResources");
            var apiResourceRecords = new List<ApiResourceRecord>();
            section.Bind(apiResourceRecords);
            List<ApiResource> apiResources = new List<ApiResource>();
            foreach (var apiResourceRecord in apiResourceRecords)
            {
                // no matter what add the api resource as a scope
                List<Scope> scopes = new List<Scope> {new Scope(apiResourceRecord.Name)};
                if (apiResourceRecord.Scopes != null)
                {
                    var prePend = string.IsNullOrWhiteSpace(apiResourceRecord.ScopeNameSpace) 
                        ? $"{apiResourceRecord.Name}."
                        : $"{apiResourceRecord.ScopeNameSpace}.";
                    foreach (var scopeRecord in apiResourceRecord.Scopes)
                    {
                        scopes.Add(new Scope($"{prePend}{scopeRecord.Name}",scopeRecord.DisplayName));
                    }
                }
                
                apiResources.Add(new ApiResource(apiResourceRecord.Name)
                {
                    Scopes = scopes
                });
            }

            return apiResources;
        }
    }
}