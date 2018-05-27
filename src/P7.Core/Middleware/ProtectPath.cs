using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class ProtectPath
    {
        
        private readonly string _policyName;

        private readonly RequestDelegate _next;
        private readonly IOptions<FiltersConfig> _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OptOutOptInFilterProvider> _logger;
        private static readonly Dictionary<string, IMiddlewarePlugin> TypeToMiddlewarePlugins = new Dictionary<string, IMiddlewarePlugin>();

        public ProtectPath(RequestDelegate next, ProtectPathOptions options, IServiceProvider serviceProvider, IOptions<FiltersConfig> settings, ILogger<OptOutOptInFilterProvider> logger)
        {
            _next = next;
            _policyName = options.PolicyName;
            _serviceProvider = serviceProvider;
            _settings = settings;
            _logger = logger;
  //          FrontLoadFilterItems();
        }

        public async Task Invoke(HttpContext httpContext, IAuthorizationService authorizationService)
        {
            var middlewarePlugins = new List<IMiddlewarePlugin>();

            foreach (var record in _settings.Value.GlobalPath.OptIn)
            {
                foreach (var path in record.Paths)
                {
                    if (httpContext.Request.Path.StartsWithSegments(path))
                    {
                        // gotcha.
                        var authorized = await authorizationService.AuthorizeAsync(
                               httpContext.User, null, _policyName);
                        if (!authorized.Succeeded)
                        {
                            await httpContext.Authentication.ChallengeAsync();
                            return;
                        }
                    }
                }
            }
            await _next(httpContext);

        }
        private IMiddlewarePlugin CreateMiddlewareInstance(string filterType)
        {
            var type = TypeHelper<Type>.GetTypeByFullName(filterType);
            var typeFilterAttribute = new TypeFilterAttribute(type) { Order = 0 };
            var filterDescriptor = new FilterDescriptor(typeFilterAttribute, 0);
            var instance = _serviceProvider.GetService(type);
            var iMiddlewarePlugin = (IMiddlewarePlugin)instance;
            return iMiddlewarePlugin;
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
                if (_settings.Value.GlobalPath.OptOut != null)
                {
                    foreach (var record in _settings.Value.GlobalPath.OptOut)
                    {
                        _logger.LogInformation("Processing OptOut Record: {0}", record);
                        try
                        {
                            middlewarePluginInstance = CreateMiddlewareInstance(record.Filter);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                $"fiter:{record.Filter}, seems to be bad, are you sure it is referenced.", e);
                        }
                        TypeToMiddlewarePlugins.Add(record.Filter, middlewarePluginInstance);
                    }
                }

                if (_settings.Value.GlobalPath.OptIn != null)
                {
                    foreach (var record in _settings.Value.GlobalPath.OptIn)
                    {
                        _logger.LogInformation("Processing OptIn Record: {0}", record);
                        try
                        {
                            middlewarePluginInstance = CreateMiddlewareInstance(record.Filter);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                $"fiter:{record.Filter}, seems to be bad, are you sure it is referenced.", e);
                        }
                        TypeToMiddlewarePlugins.Add(record.Filter, middlewarePluginInstance);
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                throw;
            }
            _logger.LogInformation("Exit");
        }
    }
}