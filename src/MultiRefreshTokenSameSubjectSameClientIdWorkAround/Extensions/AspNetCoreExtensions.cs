using Microsoft.Extensions.DependencyInjection;

namespace MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddRefreshTokenRevokationGeneratorWorkAroundTypes(this IServiceCollection services)
        {
        }
    }
}