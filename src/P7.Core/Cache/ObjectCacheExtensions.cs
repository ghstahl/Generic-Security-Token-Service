using Microsoft.Extensions.DependencyInjection;

namespace P7.Core.Cache
{
    public static class ObjectCacheExtensions
    {
        public static void AddObjectCache(this IServiceCollection services)
        {
            services.AddSingletonObjectCache();
            services.AddScopedObjectCache();
            services.AddAutoObjectCacheAllocator();
        }
        public static void AddSingletonObjectCache(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ISingletonObjectCache<,>), typeof(ObjectCache<,>));
            services.AddSingleton(typeof(ISingletonAutoObjectCache<,>), typeof(AutoObjectCache<,>));
        }

        public static void AddScopedObjectCache(this IServiceCollection services)
        {
            services.AddScoped(typeof(IScopedObjectCache<,>), typeof(ObjectCache<,>));
            services.AddScoped(typeof(IScopedAutoObjectCache<,>), typeof(AutoObjectCache<,>));
        }
        public static void AddAutoObjectCacheAllocator(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IAutoObjectCacheAllocator<,>), typeof(ObjectCacheAllocator<,>));
        }

        public static void AddScopedAutoObjectCache(this IServiceCollection services)
        {
            services.AddScoped(typeof(IScopedObjectCache<,>), typeof(ObjectCache<,>));
        }
    }
}