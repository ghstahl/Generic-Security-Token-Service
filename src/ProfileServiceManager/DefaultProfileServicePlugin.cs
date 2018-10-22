using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace ProfileServiceManager
{
    public class DefaultProfileServicePlugin: IProfileServicePlugin,IProfileService
    {
        private ILogger Logger { get; }

        public DefaultProfileServicePlugin(ILogger<DefaultProfileServicePlugin> logger)
        {
            Logger  = logger;
        }
        public string Name => "default_profile_service";
        public IProfileService ProfileService => this;
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            Logger.LogDebug("IsActive called from: {caller}", context.Caller);
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}