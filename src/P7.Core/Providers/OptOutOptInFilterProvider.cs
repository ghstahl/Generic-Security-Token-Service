using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7.Core.Middleware;
using P7.Core.Reflection;
using P7.Core.Settings;

namespace P7.Core.Providers
{
    public class FilterTypeRecord
    {
        public string Key { get; set; }
        public Type Type { get; set; }
        public ActionFilterAttribute ActionFilter { get; set; }
    }
    public interface IOptOutOptInAuthorizeStore
    {
        IEnumerable<FilterItem> GetFilterItems(string areaName, string controllerName, string actionName);
    }

    public class LocalSettingsOptOutOptInAuthorizeStore : IOptOutOptInAuthorizeStore
    {
        private IServiceProvider _serviceProvider;
        private readonly IOptions<FiltersConfig> _settings;
        private readonly ILogger<LocalSettingsOptOutOptInAuthorizeStore> _logger;
        IIndex<string, ActionFilterAttribute> _actionFilters;
        public LocalSettingsOptOutOptInAuthorizeStore(
            IServiceProvider serviceProvider,
            ILogger<LocalSettingsOptOutOptInAuthorizeStore> logger,
            IOptions<FiltersConfig> settings,
            IIndex<string, ActionFilterAttribute> actionFilters)
        {
            _settings = settings;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _actionFilters = actionFilters;
        }

        private Dictionary<string, FilterItem> _typeToFilterItem;

        private Dictionary<string, FilterItem> TypeToFilterItem
        {
            get
            {
                if (_typeToFilterItem == null)
                {
                    _logger.LogInformation("Enter");
                    try
                    {
                        if (_settings.Value.SimpleMany == null)
                        {
                            throw new Exception("_settings.Value.SimpleMany cannot be NULL.  Check your appsettings.json.");
                        }

                        List<FilterTypeRecord> filterTypes;
                        FilterItem filterItem;

                        filterTypes = new List<FilterTypeRecord>();

                        if (_settings.Value.SimpleMany.OptOut != null)
                        {
                            foreach (var record in _settings.Value.SimpleMany.OptOut)
                            {
                                _logger.LogInformation("Processing OptOut Record: {0}", record);
                                var actionFilter = _actionFilters[record.Filter];
                                if (actionFilter == null)
                                {
                                    _logger.LogCritical("IActionFilter does not exist: {0}, make sure it is registred in autofac by name", record.Filter);
                                    throw new Exception(
                                       $"fiter:{record.Filter}, seems to be bad, are you sure it is registed with autofac.");
                                }
                                var type = TypeHelper<Type>.GetTypeByFullName(record.Filter);
                                filterTypes.Add(new FilterTypeRecord() { Key = record.Filter, Type = type,ActionFilter = actionFilter});
                            }
                        }

                        if (_settings.Value.SimpleMany.OptIn != null)
                        {
                            foreach (var record in _settings.Value.SimpleMany.OptIn)
                            {
                                _logger.LogInformation("Processing OptIn Record: {0}", record);
                                var actionFilter = _actionFilters[record.Filter];
                                if (actionFilter == null)
                                {
                                    _logger.LogCritical("IActionFilter does not exist: {0}, make sure it is registred in autofac by name", record.Filter);
                                    throw new Exception(
                                       $"fiter:{record.Filter}, seems to be bad, are you sure it is registed with autofac.");
                                }
                                var type = TypeHelper<Type>.GetTypeByFullName(record.Filter);
                                filterTypes.Add(new FilterTypeRecord() { Key = record.Filter, Type = type,ActionFilter = actionFilter});
                            }
                        }
                        _typeToFilterItem = new Dictionary<string, FilterItem>();
                        foreach (var filterType in filterTypes)
                        {
                            _logger.LogInformation("Processing OptOut Record: {0}", filterType.Key);
                            try
                            {
                                var typeFilterAttribute = new TypeFilterAttribute(filterType.ActionFilter.GetType()) { Order = 0 };
                                var filterDescriptor = new FilterDescriptor(typeFilterAttribute, 0);
                                var filterMetaData = (IFilterMetadata)filterType.ActionFilter;
                                filterItem = new FilterItem(filterDescriptor, filterMetaData);

                                
                               // filterItem = _serviceProvider.CreateFilterItem(filterType.Type);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(
                                    $"fiter:{filterType.Key}, seems to be bad, are you sure it is referenced.", e);
                            }
                            _typeToFilterItem.Add(filterType.Key, filterItem);
                        }

                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e.Message);
                        _typeToFilterItem = null;
                    }
                }
                return _typeToFilterItem;
            }
        }

        public IEnumerable<FilterItem> GetFilterItems(string areaName, string controllerName, string actionName)
        {
            var filters = new List<FilterItem>();

            FilterItem filterItem;

            // the following are generic optin and optout
            foreach (var record in _settings.Value.SimpleMany.OptOut)
            {
                var match = record.RouteTree.ContainsMatch(areaName,controllerName,actionName);
                if (!match)
                {
                    filterItem = TypeToFilterItem[record.Filter];
                    filters.Add(filterItem);
                }
            }

            foreach (var record in _settings.Value.SimpleMany.OptIn)
            {
                var match = record.RouteTree.ContainsMatch(areaName, controllerName, actionName);
                if (match)
                {
                    filterItem = TypeToFilterItem[record.Filter];
                    filters.Add(filterItem);
                }
            }
            return filters;
        }
    }

    public class OptOutOptInFilterProvider : IFilterProvider
    {
        private readonly ILogger<OptOutOptInFilterProvider> _logger;
        private IOptOutOptInAuthorizeStore _authorizeStore;
        private IServiceProvider _serviceProvider;
        private static readonly object locker = new object();
        private static Dictionary<string, List<FilterItem>> ActionFilterMap = new Dictionary<string, List<FilterItem>>();
        private static Dictionary<string, FilterItem> TypeToFilterItem = new Dictionary<string, FilterItem>();

        public OptOutOptInFilterProvider(
            IServiceProvider serviceProvider,
            IOptOutOptInAuthorizeStore authorizeStore, 
            IOptions<FiltersConfig> settings,
            ILogger<OptOutOptInFilterProvider> logger)
        {
            _authorizeStore = authorizeStore;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        private List<FilterItem> FetchFilters(FilterProviderContext context)
        {
            lock (locker)
            {
                List<FilterItem> filters;
                if (!ActionFilterMap.TryGetValue(context.ActionContext.ActionDescriptor.DisplayName, out filters))
                {
                    ControllerActionDescriptor cad = (ControllerActionDescriptor)context.ActionContext.ActionDescriptor;
                    string area = (string)context.ActionContext.RouteData.Values["area"];
                    filters = _authorizeStore.GetFilterItems(area, cad.ControllerName, cad.ActionName).ToList();
                    ActionFilterMap.Add(context.ActionContext.ActionDescriptor.DisplayName, filters);
                }
                return filters;
            }
        }


        //all framework providers have negative orders, so ours will come later
        public void OnProvidersExecuting(FilterProviderContext context)
        {
            ControllerActionDescriptor cad = (ControllerActionDescriptor) context.ActionContext.ActionDescriptor;
            System.Diagnostics.Debug.WriteLine("Controller: " + cad.ControllerTypeInfo.FullName);
            System.Diagnostics.Debug.WriteLine("ActionName: " + cad.ActionName);
            System.Diagnostics.Debug.WriteLine("DisplayName: " + cad.DisplayName);
            System.Diagnostics.Debug.WriteLine("Area: " + context.ActionContext.RouteData.Values["area"]);

            var filters = FetchFilters(context);
            foreach (var filter in filters)
            {
                context.Results.Add(filter);
            }
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {
        }

        public int Order => 0;
    }
}