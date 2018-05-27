using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using P7.Core.Reflection;
using P7.Core.Settings;

namespace P7.Core.Providers
{
    public static class FilterExtensions
    {
        public static  FilterItem CreateFilterItem(this IServiceProvider serviceProvider,string filterType)
        {
            var type = TypeHelper<Type>.GetTypeByFullName(filterType);
            return serviceProvider.CreateFilterItem(type);
        }

        public static FilterItem CreateFilterItem(this IServiceProvider serviceProvider, Type filterType)
        {
            var typeFilterAttribute = new TypeFilterAttribute(filterType) { Order = 0 };
            var filterDescriptor = new FilterDescriptor(typeFilterAttribute, 0);
            var filterInstance = serviceProvider.GetService(filterType);
            var filterMetaData = (IFilterMetadata)filterInstance;
            var fi = new FilterItem(filterDescriptor, filterMetaData);
            return fi;
        }

        public static bool ContainsMatch(this List<AreaNode> routeTree, string areaName, string controllerName, string actionName)
        {
            var areaNode = routeTree.Find(x =>
            {
                if (string.IsNullOrEmpty(x.Area) && string.IsNullOrEmpty(areaName))
                    return true;
                return String.Compare(areaName, x.Area, StringComparison.OrdinalIgnoreCase) == 0;

            });
            if (areaNode != null)
            {
                if (areaNode.Controllers == null || !areaNode.Controllers.Any())
                    return true;

                var controllerNode = areaNode.Controllers.Find(x => String.Compare(controllerName, x.Controller, StringComparison.OrdinalIgnoreCase) == 0);
                if (controllerNode != null)
                {
                    if (controllerNode.Actions == null || !controllerNode.Actions.Any())
                        return true;

                    var action = controllerNode.Actions.Find(x => String.Compare(actionName, x.Action, StringComparison.OrdinalIgnoreCase) == 0);
                    return action != null;
                }
            }
            return false;
        }

        public static bool ContainsMatch(this List<AreaNode> routeTree, FilterProviderContext context)
        {
            ControllerActionDescriptor cad = (ControllerActionDescriptor)context.ActionContext.ActionDescriptor;
            string area = (string)context.ActionContext.RouteData.Values["area"];
            return routeTree.ContainsMatch(area, cad.ControllerName, cad.ActionName);
        }
    }
}
