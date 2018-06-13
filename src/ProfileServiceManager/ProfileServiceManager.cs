using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace ProfileServiceManager
{
    public class ProfileServiceManager : IProfileService
    {
        private Dictionary<string,IProfileServicePlugin> _plugins;
        private ILogger<ProfileServiceManager> _logger;

        public ProfileServiceManager(ILogger<ProfileServiceManager> logger,
            IEnumerable<IProfileServicePlugin> plugins)
        {
        //    if (_logger == null) throw new ArgumentNullException(nameof(_logger));
            if (plugins == null) throw new ArgumentNullException(nameof(plugins));

            _plugins = new Dictionary<string, IProfileServicePlugin>();
            _logger = logger;
            foreach (var plugin in plugins)
            {
                _plugins.Add(plugin.Name,plugin);
            }
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var query = from item in context.Subject.Claims
                where item.Type == Constants.ClaimKey
                        select item.Value;
            var pluginKey = query.FirstOrDefault();
            if (pluginKey != null)
            {
                if (_plugins.ContainsKey(pluginKey))
                {
                    var plugin = _plugins[pluginKey];
                    await plugin.ProfileService.GetProfileDataAsync(context);
                    return;
                }
            }
            else
            {
                pluginKey = "default";
                if (_plugins.ContainsKey(pluginKey))
                {
                    var plugin = _plugins[pluginKey];
                    await plugin.ProfileService.GetProfileDataAsync(context);
                }
            }
            throw new Exception($"{Constants.ClaimKey} is not present, or does not reference a plugin.");
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var query = from item in context.Subject.Claims
                where item.Type == Constants.ClaimKey
                select item.Value;
            var pluginKey = query.FirstOrDefault();
            if (pluginKey != null)
            {
                if (_plugins.ContainsKey(pluginKey))
                {
                    var plugin = _plugins[pluginKey];
                    await plugin.ProfileService.IsActiveAsync(context);
                }
            }
            else
            {
                pluginKey = "default";
                if (_plugins.ContainsKey(pluginKey))
                {
                    var plugin = _plugins[pluginKey];
                    await plugin.ProfileService.IsActiveAsync(context);
                }
            }
        }
    }
}
