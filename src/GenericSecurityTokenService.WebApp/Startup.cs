using System;
using System.Collections.Generic;
using System.Linq;
using ArbitraryIdentityExtensionGrant.Extensions;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using GenericSecurityTokenService.Middleware;
using GenericSecurityTokenService.Services;
using IdentityModelExtras.Extensions;
using IdentityServer4.Contrib.RedisStoreExtra.Extenstions;
using IdentityServer4.Hosting;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7.Core;
using P7.Core.Scheduler.Scheduling;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace GenericSecurityTokenService
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        private readonly IHostingEnvironment _hostingEnvironment;
        private IStartupConfigurationService _externalStartupConfiguration;

        public Startup(
            IConfiguration configuration, 
            IHostingEnvironment hostingEnvironment, 
            IStartupConfigurationService externalStartupConfiguration)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _externalStartupConfiguration = externalStartupConfiguration;
            _externalStartupConfiguration.ConfigureEnvironment(hostingEnvironment);
            StartupConfiguration(configuration);
            StartupLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    corsBuilder => corsBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            var clients = Configuration.LoadClientsFromSettings();
            var apiResources = Configuration.LoadApiResourcesFromSettings();
            var identityResources = Configuration.LoadIdentityResourcesFromSettings();
            bool useRedis = Convert.ToBoolean(Configuration["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVault"]);
            var builder = services
                .AddIdentityServer(options =>
                {
                    options.InputLengthRestrictions.RefreshToken = 256;
                })
                .AddInMemoryIdentityResources(identityResources)
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClientsExtra(clients)
                .AddIdentityServer4Extras()
                .AddProfileServiceManager()
                .AddArbitraryOwnerResourceExtensionGrant()
                .AddArbitraryIdentityExtensionGrant()
                .AddArbitraryNoSubjectExtensionGrant();
            // My Replacement Services.
            if (useRedis)
            {
                var redisConnectionString = Configuration["appOptions:redis:redisConnectionString"];
                builder.AddOperationalStore(options =>
                    {
                        options.RedisConnectionString = redisConnectionString;
                        options.Db = 1;
                    })
                    .AddRedisCaching(options =>
                    {
                        options.RedisConnectionString = redisConnectionString;
                        options.KeyPrefix = "prefix";
                    });
                builder.AddRedisOperationalStoreExtra();
                services.AddRedisOperationalStoreExtraTypes();

            }
            else
            {
                builder.AddInMemoryPersistedGrantStoreExtra();
            }
            if (useKeyVault)
            {
                builder.AddKeyVaultTokenCreateService();
                services.AddKeyVaultTokenCreateServiceTypes();
                services.AddKeyVaultTokenCreateServiceConfiguration(Configuration);
            }
            else
            {
                builder.AddDeveloperSigningCredential();
                if (_hostingEnvironment.IsDevelopment())
                {
                    builder.AddDeveloperSigningCredential();
                }
                else
                {
                    //crash
                }
            } // my replacement services.
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

            // my configurations
            services.AddSingleton<IHostedService, SchedulerHostedService>();
            services.Configure<Options.RedisAppOptions>(Configuration.GetSection("appOptions:redis"));
            services.Configure<Options.KeyVaultAppOptions>(Configuration.GetSection("appOptions:keyVault"));
            services.RegisterP7CoreConfigurationServices(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            /*
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["self:authority"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudiences = new List<string>()
                        {
                            $"{options.Authority}/Resouces"
                        }
                    };
                });
                */
            services.AddLogging();
            // Pass configuration (IConfigurationRoot) to the configuration service if needed
            _externalStartupConfiguration.ConfigureService(services, null);

            var identityServer4BasePath = Configuration["IdentityServerPublicFacingUri"];
            if (!string.IsNullOrEmpty(identityServer4BasePath))
            {
                identityServer4BasePath = identityServer4BasePath.Trim('/');
                var endpoints = builder.Services
                    .Where(service => service.ServiceType == typeof(Endpoint))
                    .Select(item => (Endpoint)item.ImplementationInstance)
                    .ToList();

                // endpoints.ForEach(item =>item.Path.Value.r = $"api/Authority/{item.Path.Value}");
                endpoints.ForEach(item => item.Path = item.Path.Value.Replace("connect", $"{identityServer4BasePath}/connect"));
                endpoints.ForEach(item => item.Path = item.Path.Value.Replace(".well-known/openid-configuration", 
                    $"{identityServer4BasePath}/.well-known/openid-configuration"));

            }
 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _externalStartupConfiguration.Configure(app, env, loggerFactory);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<PublicFacingUrlMiddleware>();
           
            app.UseIdentityServer();
            
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
        private void StartupConfiguration(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.redis.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.keyVault.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.IdentityResources.json", optional: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.ApiResources.json", optional: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.Clients.json", optional: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.json", optional: true);

            if (_hostingEnvironment.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            PublicFacingUrlMiddleware.PathRootUrl = Configuration["IdentityServerPublicFacingUri"];
        }
        private void StartupLogger()
        { 
        }
    }
}
