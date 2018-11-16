using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ArbitraryIdentityExtensionGrant.Extensions;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using AspNetCoreRateLimit;
using GraphQLCore.ExtensionGrants.Extensions;
using IdentityModelExtras.Extensions;
using IdentityServer4.Contrib.RedisStoreExtra.Extenstions;
using IdentityServer4.HostApp.Health;
using IdentityServer4.HostApp.RateLimiting;
using IdentityServer4.Stores;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using IdentityServer4Extras.Stores;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7.Core;
using P7.Core.Scheduler.Scheduling;
using P7.GraphQLCore;
using P7.GraphQLCore.Extensions;
using P7.GraphQLCore.Stores;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Westwind.AspNetCore.Markdown;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
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

            services.AddSingleton<OIDCDiscoverCacheContainer>();

            bool useRedis = Convert.ToBoolean(Configuration["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVault"]);
            var builder = services
                .AddIdentityServer(options => { options.InputLengthRestrictions.RefreshToken = 256; })
                .AddInMemoryIdentityResources(identityResources)
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClientsExtra(clients)
                .AddTestUsers(Config.GetUsers())
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

            builder.AddPluginHostClientSecretValidator();
            builder.AddNoSecretRefreshClientSecretValidator();

            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra
            builder.SwapOutTokenResponseGenerator();
            builder.SwapOutDefaultTokenService();
            builder.SwapOutScopeValidator();
            builder.SwapOutTokenRevocationRequestValidator();

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryIdentityExtentionGrantTypes();
            services.AddIdentityModelExtrasTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            builder.AddProtectedRefreshTokenKeyObfuscator();
            builder.Services.TryAddSingleton<IGraphQLFieldAuthority, InMemoryGraphQLFieldAuthority>();

            services.AddGraphQLCoreTypes();
            services.AddGraphQLCoreExtensionGrantsTypes();

            services.AddRateLimiting(Configuration);

            // my configurations
            services.AddSingleton<IHostedService, SchedulerHostedService>();
            services.Configure<Options.RedisAppOptions>(Configuration.GetSection("appOptions:redis"));
            services.Configure<Options.KeyVaultAppOptions>(Configuration.GetSection("appOptions:keyVault"));
            services.RegisterP7CoreConfigurationServices(Configuration);
            services.RegisterGraphQLCoreConfigurationServices(Configuration);

            services.AddMyHealthCheck(Configuration);

         
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    return apiDesc.HttpMethod != null;
                });
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Nudibranch API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Slammin Rammen",
                        Email = "admin@supercorp.com",
                        Url = "https://twitter.com/slamminrammen"
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });
                // TODO: fix this by copying over the XML file
                /*
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                */
            });


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
            // Enable middleware to serve generated Swagger as a JSON endpoint.

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

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
            app.UseIdentityServerClientMiddleware();
            app.UseIdentityServer();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();

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
                .AddJsonFile("appsettings.graphql.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.ratelimiting.json", optional: true)
                .AddJsonFile($"appsettings.{_hostingEnvironment.EnvironmentName}.healthcheck.json", optional: true)
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
        }
    }
}
