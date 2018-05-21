using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4Extras.Extensions
{
    public static class ApiResourceSettingsExtensions
    {
        public static List<ApiResource> LoadApiResourcesFromSettings(this IConfiguration configuration)
        {

            IConfigurationSection section = configuration.GetSection("apiResources");
            var apiResourceSettings = new List<string>();
            section.Bind(apiResourceSettings);
            List<ApiResource> apiResources = new List<ApiResource>();
            foreach (var apiResourceSetting in apiResourceSettings)
            {
                apiResources.Add(new ApiResource(apiResourceSetting));
            }

            return apiResources;
        }
    }
}