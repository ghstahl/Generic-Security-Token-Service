using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P7.Core.Providers;
using P7.Core.Reflection;
using P7.Core.Settings;

namespace P7.Core.Middleware
{
    public interface IGlobalPathAuthorizeStore
    {
        IEnumerable<FilterTypeRecord> GetFilterTypes();
    }

    public class LocalSettingsGlobalPathAuthorizeStore : IGlobalPathAuthorizeStore
    {
        private readonly IOptions<FiltersConfig> _settings;
        private readonly ILogger<LocalSettingsGlobalPathAuthorizeStore> _logger;
        public LocalSettingsGlobalPathAuthorizeStore(
            ILogger<LocalSettingsGlobalPathAuthorizeStore> logger,
            IOptions<FiltersConfig> settings)
        {
            _settings = settings;
            _logger = logger;
        }

        private List<FilterTypeRecord> _filterTypes;
        public IEnumerable<FilterTypeRecord> GetFilterTypes()
        {
            _logger.LogInformation("Enter");
            try
            {
                if (_filterTypes == null)
                {
                    _filterTypes = new List<FilterTypeRecord>();
                    if (_settings.Value.GlobalPath.OptOut != null)
                    {
                        foreach (var record in _settings.Value.GlobalPath.OptOut)
                        {
                            _logger.LogInformation("Processing OptOut Record: {0}", record);
                            var type = TypeHelper<Type>.GetTypeByFullName(record.Filter);
                            _filterTypes.Add(new FilterTypeRecord() {Key = record.Filter,Type = type});
                        }
                    }
                    if (_settings.Value.GlobalPath.OptIn != null)
                    {
                        foreach (var record in _settings.Value.GlobalPath.OptIn)
                        {
                            _logger.LogInformation("Processing OptIn Record: {0}", record);
                            var type = TypeHelper<Type>.GetTypeByFullName(record.Filter);
                            _filterTypes.Add(new FilterTypeRecord() { Key = record.Filter, Type = type });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _filterTypes = null;
                _logger.LogCritical(e.Message );
            }
            _logger.LogInformation("Exit");
            return _filterTypes;
        }
    }

    public class AuthorizeMiddleware
    {
        private IGlobalPathAuthorizeStore _authorizeStore;
        private readonly RequestDelegate _next;
        private readonly IOptions<FiltersConfig> _settings;
        private IServiceProvider _serviceProvider;
        private readonly ILogger<OptOutOptInFilterProvider> _logger;
        private static Dictionary<string, IMiddlewarePlugin> TypeToMiddlewarePlugins = new Dictionary<string, IMiddlewarePlugin>();

        public AuthorizeMiddleware(RequestDelegate next,
            IServiceProvider serviceProvider,
            IGlobalPathAuthorizeStore authorizeStore,
            IOptions<FiltersConfig> settings, ILogger<OptOutOptInFilterProvider> logger)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _settings = settings;
            _logger = logger;
            _authorizeStore = authorizeStore;
            FrontLoadFilterItems();
        }

      
        private void FrontLoadFilterItems()
        {
            _logger.LogInformation("Enter");
            try
            {
                if (_settings.Value.GlobalPath == null)
                {
                    throw new Exception("_settings.Value.GlobalPath cannot be NULL.  Check your appsettings.json.");
                }

                IMiddlewarePlugin middlewarePluginInstance;
                var filterTypes = _authorizeStore.GetFilterTypes();
                foreach (var filterType in filterTypes)
                {
                    _logger.LogInformation("Processing filterType: {0}", filterType.Key);
                    try
                    {
                        middlewarePluginInstance = _serviceProvider.CreateMiddlewareInstance(filterType.Type);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            $"fiter:{filterType.Key}, seems to be bad, are you sure it is referenced.", e);
                    }
                    TypeToMiddlewarePlugins.Add(filterType.Key, middlewarePluginInstance);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                throw;
            }
            _logger.LogInformation("Exit");
        }
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Method != "GET")
            {
                await _next(httpContext);
                return;
            }
            var middlewarePlugins = new List<IMiddlewarePlugin>();

            foreach (var record in _settings.Value.GlobalPath.OptIn)
            {
                foreach (var strRegex in record.Paths)
                {

                    Regex myRegex = new Regex(strRegex, RegexOptions.IgnoreCase);
                    string strTargetString = httpContext.Request.Path;
                    var match = myRegex.Matches(strTargetString).Cast<Match>().Any(myMatch => myMatch.Success);
                    if (match)
                    {
                        middlewarePlugins.Add(TypeToMiddlewarePlugins[record.Filter]);
                        break;
                    }
                }
            }
            bool continueToNext = true;

            foreach (var mp in middlewarePlugins)
            {
                continueToNext = mp.Invoke(httpContext);
                if (!continueToNext)
                {
                    break;
                }
            }


            if (continueToNext)
            {
                await _next(httpContext);
            }
        }
    }
}