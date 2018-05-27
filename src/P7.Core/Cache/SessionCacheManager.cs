using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace P7.Core.Cache
{
    public static class SessionCacheManager<T> where T : class
    {
        public static T Grab(HttpContext httpContext, string cacheKey)
        {
            if (httpContext == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return null;
            }
            if (httpContext.Session == null)
            {
                return null;
            }
            var myComplexObject = httpContext.Session.GetObjectFromJson<T>(cacheKey);
            return myComplexObject;
        }

        public static void Insert(HttpContext httpContext, string cacheKey, T obj)
        {
            httpContext.Session?.SetObjectAsJson(cacheKey, obj);
        }

        public static void RemoveCache(HttpContext httpContext, string cacheKey)
        {
            httpContext.Session?.Remove(cacheKey);
        }
    }
}
