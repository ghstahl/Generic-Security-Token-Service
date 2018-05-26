using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryOpenIdConnectTokenExtensionGrants.Extensions;
using ArbitraryResourceOwnerExtensionGrant;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using IdentityServer4.Contrib.RedisStoreExtra.Extenstions;
using IdentityServer4.Models;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServer4Extras.Models;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using Newtonsoft.Json;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;
using Serilog;
using Westwind.AspNetCore.Markdown;

namespace IdentityServer4.HostApp.Redis
{

    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; private set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            StartupConfiguration(configuration);
            StartupLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var clients = Configuration.LoadClientsFromSettings();
            var apiResources = Configuration.LoadApiResourcesFromSettings();

            services.AddSingleton<OIDCDiscoverCacheContainer>();

            var builder = services
                .AddIdentityServer()
             //   .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClientsExtra(clients)
                .AddTestUsers(Config.GetUsers())
                .AddIdentityServer4Extras()
                .AddProfileServiceManager()
                .AddArbitraryOwnerResourceExtensionGrant()
                .AddArbitraryOpenIdConnectTokenExtensionGrant()
                .AddArbitraryNoSubjectExtensionGrant()
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionString = Configuration["redisConnectionString"];
                    options.Db = 1;
                })
                .AddRedisCaching(options =>
                {
                    options.RedisConnectionString = Configuration["redisConnectionString"];
                    options.KeyPrefix = "prefix";
                });

            if (_hostingEnvironment.IsDevelopment())
            {
               
            }
            else
            {
               
            }

            // my replacement services.

            builder.AddRefreshTokenRevokationGeneratorWorkAround();
            builder.AddNoSecretRefreshClientSecretValidator();
            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra
            builder.AddRedisOperationalStoreExtra();
            builder.AddKeyVaultTokenCreateService();

            // My Types
            services.AddKeyVaultTokenCreateServiceTypes();
            services.AddRedisOperationalStoreExtraTypes();
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryOpenIdConnectTokenExtensionGrantTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            // my configurations
            services.AddKeyVaultTokenCreateServiceConfiguration(Configuration);

            services.AddMemoryCache();
            services.AddMvc();

            services.AddLogging();
            services.AddWebEncoders();

            services.AddMarkdown(config =>
            {
                config.AddMarkdownProcessingFolder("/docs/");
                // Create custom MarkdigPipeline 
                // using MarkDig; for extension methods
                config.ConfigureMarkdigPipeline = pipeLineBuilder =>
                {
                    pipeLineBuilder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UsePipeTables()
                        .UseGridTables()
                        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                        .UseAutoLinks() // URLs are parsed into anchors
                        .UseAbbreviations()
                        .UseYamlFrontMatter()
                        .UseEmojiAndSmiley(true)
                        .UseListExtras()
                        .UseFigures()
                        .UseTaskLists()
                        .UseCustomContainers()
                        .UseGenericAttributes();
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseMarkdown();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvc();
        }
        private void StartupLogger()
        {
            var appDataPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");

            var rollingPath = Path.Combine(_hostingEnvironment.ContentRootPath, "logs/myapp-{Date}.txt");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(rollingPath)
                .WriteTo.LiterateConsole()
                .CreateLogger();
            Log.Information("Ah, there you are!");
        }

        private void StartupConfiguration(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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
        }
    }
}
