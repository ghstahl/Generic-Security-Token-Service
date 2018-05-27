using Microsoft.Extensions.DependencyInjection;

namespace HealthCheck.Core.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddHealthCheckCoreTypes(this IServiceCollection services)
        {
            //  services.AddTransient<{Some Type}>();
            services.AddTransient<AggregateHealthCheck>();
            services.AddSingleton<IHealthCheckStore, HealthCheckStore>();
        }
    }
}