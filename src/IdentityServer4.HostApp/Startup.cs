using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryOpenIdConnectTokenExtensionGrants;
using ArbitraryOpenIdConnectTokenExtensionGrants.Extensions;
using ArbitraryResourceOwnerExtensionGrant;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using IdentityModelExtras.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using ProfileServiceManager.Extensions;
using Serilog;
using Westwind.AspNetCore.Markdown;

namespace IdentityServer4.HostApp
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

            var builder = services.AddIdentityServer(options => { options.InputLengthRestrictions.RefreshToken = 2048; })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClientsExtra(clients)
                .AddTestUsers(Config.GetUsers())
                .AddIdentityServer4Extras()
                .AddProfileServiceManager()
                .AddArbitraryOwnerResourceExtensionGrant()
                .AddArbitraryOpenIdConnectTokenExtensionGrant()
                .AddArbitraryNoSubjectExtensionGrant();

            // My Replacement Services.
            builder.AddRefreshTokenRevokationGeneratorWorkAround();
            builder.AddNoSecretRefreshClientSecretValidator();
            builder.AddInMemoryClientStoreExtra();// InMemoryPersistedGrantStoreExtra extra needs IClientStoreExtra
            builder.AddInMemoryPersistedGrantStoreExtra();

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryOpenIdConnectTokenExtensionGrantTypes();
            services.AddIdentityModelExtrasTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

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
                        pipeLineBuilder
                            .UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
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
