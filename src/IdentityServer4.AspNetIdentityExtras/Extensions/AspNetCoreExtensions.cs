using Microsoft.Extensions.DependencyInjection;
using ProfileServiceManager;

namespace IdentityServer4.AspNetIdentityExtras.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddAspNetIdentityExtrasTypes<TUser>(this IServiceCollection services)
            where TUser : class
        {
            services.AddTransient<IProfileServicePlugin, AspNetIdentityProfileServiceExtra<TUser>>();
        }
    }
}