using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using GraphQLCore.ExtensionGrants.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.AspNetFederatedHost.Data;
using IdentityServer4.AspNetFederatedHost.Services;
using IdentityServer4.AspNetIdentityExtras.Extensions;
using IdentityServer4.Contrib.RedisStoreExtra.Extenstions;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using P7.Core;
using P7.GraphQLCore;
using P7.GraphQLCore.Extensions;
using P7.GraphQLCore.Stores;
using P7IdentityServer4.Extensions;
using ProfileServiceManager.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace IdentityServer4.AspNetFederatedHost
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            StartupConfiguration(configuration);
            StartupLogger();
        }

        public IConfiguration Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Name = $"{Configuration["applicationName"]}.AspNetCore.Consent";
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = $"{Configuration["applicationName"]}.AspNetCore.Identity.Application";
            });
            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var clients = Configuration.LoadClientsFromSettings();
            var apiResources = Configuration.LoadApiResourcesFromSettings();
            var identityResources = Configuration.LoadIdentityResourcesFromSettings();
            
            bool useRedis = Convert.ToBoolean(Configuration["appOptions:redis:useRedis"]);
            bool useKeyVault = Convert.ToBoolean(Configuration["appOptions:keyVault:useKeyVault"]);
            // configure identity server with in-memory stores, keys, clients and scopes
            var builder = services.AddIdentityServer(options => { options.UserInteraction.LoginUrl = "/identity/account/login"; })
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(identityResources)
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryClients(clients)
                .AddIdentityServer4Extras()
                .AddArbitraryOwnerResourceExtensionGrant()
                .AddArbitraryNoSubjectExtensionGrant()
                .AddAspNetIdentity<IdentityUser>();

            
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
            if (useKeyVault)
            {
                builder.AddKeyVaultTokenCreateService();
                services.AddKeyVaultTokenCreateServiceTypes();
                services.AddKeyVaultTokenCreateServiceConfiguration(Configuration);
            }
            else
            {
                builder.AddDeveloperSigningCredential();
            }
            // my replacement services.
            builder.AddProfileServiceManager();
            builder.AddRefreshTokenRevokationGeneratorWorkAround();
            
            builder.AddPluginHostClientSecretValidator();
            builder.AddNoSecretRefreshClientSecretValidator();

            builder.AddInMemoryClientStoreExtra(); // redis extra needs IClientStoreExtra

            // My Types
            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();
            services.AddAspNetIdentityExtrasTypes<IdentityUser>();
            

            builder.Services.TryAddSingleton<IGraphQLFieldAuthority, InMemoryGraphQLFieldAuthority>();

            services.AddGraphQLCoreTypes();
            services.AddGraphQLCoreExtensionGrantsTypes();

            // my configurations
            
            services.Configure<Options.RedisAppOptions>(Configuration.GetSection("appOptions:redis"));
            services.Configure<Options.KeyVaultAppOptions>(Configuration.GetSection("appOptions:keyVault"));
            services.RegisterP7CoreConfigurationServices(Configuration);
            services.RegisterGraphQLCoreConfigurationServices(Configuration);

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

            services.AddTransient<IEmailSender, EmailSender>();

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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
        private void StartupConfiguration(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.redis.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.keyVault.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.graphql.json", optional: false, reloadOnChange: true)
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
    }
}
