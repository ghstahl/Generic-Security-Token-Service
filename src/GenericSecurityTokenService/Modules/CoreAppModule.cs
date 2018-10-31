using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using ArbitraryIdentityExtensionGrant.Extensions;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using GenericSecurityTokenService.Functions;
using IdentityModelExtras.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;

namespace GenericSecurityTokenService.Modules
{
    public class MyHttpContextAccessor : IHttpContextAccessor
    {
        private static AsyncLocal<(string traceIdentifier, HttpContext context)> _httpContextCurrent 
            = new AsyncLocal<(string traceIdentifier, HttpContext context)>();

        public HttpContext HttpContext
        {
            get
            {
                var value = _httpContextCurrent.Value;
                // Only return the context if the stored request id matches the stored trace identifier
                // context.TraceIdentifier is cleared by HttpContextFactory.Dispose.
                return value.traceIdentifier == value.context?.TraceIdentifier ? value.context : null;
            }
            set
            {
                _httpContextCurrent.Value = (value?.TraceIdentifier, value);
            }
        }
    }

    public class MyContext
    {
        public string TraceIdentifier { get; set; }
        private Dictionary<string, string> _dictionary;

        public Dictionary<string, string> Dictionary
        {
            get { return _dictionary ?? (_dictionary = new Dictionary<string, string>()); }
        }
    }
    public interface IMyContextAccessor
    {
        MyContext MyContext { get; set; }
    }
    public class MyContextAccessor : IMyContextAccessor
    {
        private static AsyncLocal<(string traceIdentifier, MyContext context)> _contextCurrent
            = new AsyncLocal<(string traceIdentifier, MyContext context)>();

        public MyContext MyContext
        {
            get
            {
                var value = _contextCurrent.Value;
                // Only return the context if the stored request id matches the stored trace identifier
                // context.TraceIdentifier is cleared by HttpContextFactory.Dispose.
                return value.traceIdentifier == value.context?.TraceIdentifier ? value.context : null;
            }
            set
            {
                _contextCurrent.Value = (value?.TraceIdentifier, value);
            }
        }
    }

    public class MyHostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }

     
 
    /// <summary>
    /// This represents the module entity for dependencies.
    /// </summary>
    public class CoreAppModule : Module
    {
        private string _basePath;
        public CoreAppModule(string basePath)
        {
            _basePath = basePath;
        }
        /// <inheritdoc />
        public override void Load(IServiceCollection services)
        {
            var configurationBuilderEnvironment = new ConfigurationBuilder()
                .SetBasePath(_basePath)
                .AddEnvironmentVariables();
            var configEnvironment = configurationBuilderEnvironment.Build();
            var environmentName = configEnvironment["ASPNETCORE_ENVIRONMENT"];
            IHostingEnvironment hostingEnvironment = new MyHostingEnvironment()
            {
                ApplicationName = "GenericSecurityTokenService",
                EnvironmentName = environmentName
            };
            services.AddSingleton<IHostingEnvironment>(hostingEnvironment);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(_basePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"appsettings.IdentityResources.json")
                .AddJsonFile($"appsettings.ApiResources.json")
                .AddJsonFile($"appsettings.keyVault.json")
                .AddJsonFile($"appsettings.Clients.json");
            if (hostingEnvironment.IsDevelopment())
            {
                configurationBuilder.AddUserSecrets<CoreAppModule>();
            }
            configurationBuilder.AddEnvironmentVariables();
            var config = configurationBuilder.Build();

            bool useRedis = Convert.ToBoolean(config["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(config["appOptions:keyVault:useKeyVault"]);

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHelloFunction, CoreHelloFunction>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddTransient<ILoggerFactory, LoggerFactory>();

            var clients = config.LoadClientsFromSettings();
            var apiResources = config.LoadApiResourcesFromSettings();
            var identityResources = config.LoadIdentityResourcesFromSettings();

            var builder = services
                .AddIdentityServer(options => { options.InputLengthRestrictions.RefreshToken = 256; })
                .AddInMemoryIdentityResources(identityResources)
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClientsExtra(clients)
                .AddIdentityServer4Extras()
                .AddProfileServiceManager()
                .AddArbitraryOwnerResourceExtensionGrant()
                .AddArbitraryIdentityExtensionGrant()
                .AddArbitraryNoSubjectExtensionGrant();

            builder.AddInMemoryPersistedGrantStoreExtra();

            if (useKeyVault)
            {
                builder.AddKeyVaultTokenCreateService();
                services.AddKeyVaultTokenCreateServiceTypes();
                services.AddKeyVaultTokenCreateServiceConfiguration(config);
            }
            else
            {
                builder.AddDeveloperSigningCredential();
            }


            // my replacement services.
            builder.AddRefreshTokenRevokationGeneratorWorkAround();
            builder.AddNoSecretRefreshClientSecretValidator();
            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra
            builder.SwapOutTokenResponseGenerator();
            builder.SwapOutDefaultTokenService();
            builder.SwapOutScopeValidator();

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryIdentityExtentionGrantTypes();
            services.AddIdentityModelExtrasTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            builder.AddProtectedRefreshTokenKeyObfuscator();

            var endpoints = builder.Services
                .Where(service => service.ServiceType == typeof(Endpoint))
                .Select(item => (Endpoint)item.ImplementationInstance)
                .ToList();
            // endpoints.ForEach(item =>item.Path.Value.r = $"api/Authority/{item.Path.Value}");
            endpoints.ForEach(item => item.Path = item.Path.Value.Replace("connect", "api/Authority/connect"));
            endpoints.ForEach(item => item.Path = item.Path.Value.Replace(".well-known/openid-configuration", "api/Authority/.well-known/openid-configuration"));//



            services.AddMemoryCache();
            services.AddSingleton<IHttpContextAccessor, MyHttpContextAccessor>();
            services.AddSingleton<IMyContextAccessor, MyContextAccessor>();

        }
    }
}
