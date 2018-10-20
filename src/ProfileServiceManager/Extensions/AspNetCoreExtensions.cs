using Microsoft.Extensions.DependencyInjection;

namespace ProfileServiceManager.Extensions
{
    public static class AspNetCoreExtensions
    {
 
        public static void AddProfileServiceManagerTypes(this IServiceCollection services)
        {
            //  services.AddTransient<{Some Type}>();
            services.AddTransient<ProfileServiceManager>();
            services.AddTransient<IProfileServicePlugin, DefaultProfileServicePlugin>();
        }
    }
}