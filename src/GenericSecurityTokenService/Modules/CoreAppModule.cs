using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using ArbitraryIdentityExtensionGrant.Extensions;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using GenericSecurityTokenService.Functions;
using GenericSecurityTokenService.Security;
using IdentityModelExtras.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;

namespace GenericSecurityTokenService.Modules
{
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

            services.AddSingleton<IConfiguration>(config);

            bool useRedis = Convert.ToBoolean(config["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(config["appOptions:keyVault:useKeyVault"]);

            services.AddSingleton<HttpClient>();

            services.AddSingleton<IAuthorityFunction, CoreAuthorityFunction>();
            services.AddSingleton<IIdentityFunction, CoreIdentityFunction>();
         
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
            services.AddSingleton<IFunctionHttpContextAccessor, MyHttpContextAccessor>();
            services.AddSingleton<IHttpContextAccessor>(s => s.GetService<IFunctionHttpContextAccessor>());
            services.AddSingleton<IMyContextAccessor, MyContextAccessor>();
            services.AddSingleton<ITokenValidator, TokenValidator>();
        }
    }
}
