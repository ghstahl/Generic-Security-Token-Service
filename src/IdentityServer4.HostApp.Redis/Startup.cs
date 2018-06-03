using System;
using System.IO;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryOpenIdConnectTokenExtensionGrants.Extensions;
using ArbitraryResourceOwnerExtensionGrant;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using IdentityServer4.Contrib.RedisStoreExtra.Extenstions;
using IdentityServer4.HostApp.Redis.Health;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7.Core.Scheduler.Scheduling;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;
using Serilog;
using Westwind.AspNetCore.Markdown;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var clients = Configuration.LoadClientsFromSettings();
            var apiResources = Configuration.LoadApiResourcesFromSettings();

            services.AddSingleton<OIDCDiscoverCacheContainer>();

            bool useRedis = Convert.ToBoolean(Configuration["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVault"]);
            var builder = services
                .AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClientsExtra(clients)
                .AddTestUsers(Config.GetUsers())
                .AddIdentityServer4Extras()
                .AddProfileServiceManager()
                .AddArbitraryOwnerResourceExtensionGrant()
                .AddArbitraryOpenIdConnectTokenExtensionGrant()
                .AddArbitraryNoSubjectExtensionGrant();
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

            if (_hostingEnvironment.IsDevelopment())
            {
               
            }
            else
            {
               
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
            }

            // my replacement services.

            builder.AddRefreshTokenRevokationGeneratorWorkAround();
            builder.AddNoSecretRefreshClientSecretValidator();
            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryOpenIdConnectTokenExtensionGrantTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            // my configurations
            services.AddSingleton<IHostedService, SchedulerHostedService>();
            services.Configure<Options.RedisAppOptions>(Configuration.GetSection("appOptions:redis"));
            services.Configure<Options.KeyVaultAppOptions>(Configuration.GetSection("appOptions:keyVault"));


            services.AddMyHealthCheck(Configuration);
            services.AddMemoryCache();
            services.AddMvc();

            services.AddLogging();
            services.AddWebEncoders();
            if (Convert.ToBoolean(Configuration["addMarkdown"]))
            {
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
                            .UseBootstrap()
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
            // Build the intermediate service provider then return it
            return services.BuildServiceProvider();
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

            if (Convert.ToBoolean(Configuration["addMarkdown"]))
            {
                app.UseMarkdown();
            }

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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.redis.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.keyVault.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.ratelimiting.json", optional: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.healthcheck.json", optional: true)
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
