using Microsoft.Extensions.DependencyInjection;

namespace P7.Core.Cache
{
    public static class ObjectCacheExtensions
    {
        public static void AddObjectCache(this IServiceCollection services)
        {
            services.AddSingletonObjectCache();
            services.AddScopedObjectCache();
        }
        public static void AddSingletonObjectCache(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ISingletonObjectCache<,>), typeof(ObjectCache<,>));
        }
        
        public static void AddScopedObjectCache(this IServiceCollection services)
        {
            services.AddScoped(typeof(IScopedObjectCache<,>), typeof(ObjectCache<,>));
        }
    }
}