using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using P7.Core.Reflection;

namespace P7.Core.Middleware
{
    public static class MiddleWareExtensions
    {
        // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
        // Extension method used to add the middleware to the HTTP request pipeline.
        public static IApplicationBuilder UseAuthorizeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizeMiddleware>();
        }

        public static IMiddlewarePlugin CreateMiddlewareInstance(this IServiceProvider serviceProvider,string filterType)
        {
            var type = TypeHelper<Type>.GetTypeByFullName(filterType);
            return serviceProvider.CreateMiddlewareInstance(type);
        }

        public static IMiddlewarePlugin CreateMiddlewareInstance(this IServiceProvider serviceProvider, Type filterType)
        {
            var typeFilterAttribute = new TypeFilterAttribute(filterType) { Order = 0 };
            var filterDescriptor = new FilterDescriptor(typeFilterAttribute, 0);
            var instance = serviceProvider.GetService(filterType);
            var iMiddlewarePlugin = (IMiddlewarePlugin)instance;
            return iMiddlewarePlugin;
        }
    }
}
