using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ZeroFormatter;

namespace P7.Core.Cache
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetObjectAsJson<T>(this IDistributedCache cache,string key, T item, 
            int expirationInMinutes)
        {
            var json = JsonConvert.SerializeObject(item);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
            });
        }
        public static async Task<T> GetObjectFromJson<T>(this IDistributedCache cache,string key)
        {
            var json = await cache.GetStringAsync(key);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }
        public static async Task SetObjectAsZeroFormatter<T>(this IDistributedCache cache, string key, T item,
            int expirationInMinutes)
        {
            var val = ZeroFormatterSerializer.Serialize(item);
            await cache.SetAsync(key, val, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
            });
        }
        public static async Task<T> GetObjectFromZeroFormatter<T>(this IDistributedCache cache, string key)
        {
            
            var value = await cache.GetAsync(key);
            if (value != null)
            {
                var obj = ZeroFormatterSerializer.Deserialize<T>(value);
                return obj;
            }
            return default(T);

        }
    }
}