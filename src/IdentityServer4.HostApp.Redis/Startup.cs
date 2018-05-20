using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArbitraryNoSubjectExtensionGrant.Extensions;
using ArbitraryOpenIdConnectTokenExtensionGrants.Extensions;
using ArbitraryResourceOwnerExtensionGrant;
using ArbitraryResourceOwnerExtensionGrant.Extensions;
using IdentityServer4.Models;
using IdentityServer4Extras;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions;
using Newtonsoft.Json;
using ProfileServiceManager.Extensions;
using Serilog;

namespace IdentityServer4.HostApp.Redis
{
 
    public partial class ClientRecord
    {
        [JsonIgnore]
        public string ClientId { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("secrets")]
        public List<string> Secrets { get; set; }

        [JsonProperty("allowedScopes")]
        public List<string> AllowedScopes { get; set; }

        [JsonProperty("AllowedGrantTypes")]
        public List<string> AllowedGrantTypes { get; set; }

        [JsonProperty("AccessTokenLifetime")]
        public int AccessTokenLifetime { get; set; }
 
        [JsonProperty("AbsoluteRefreshTokenLifetime")]
        public int AbsoluteRefreshTokenLifetime { get; set; }

        [JsonProperty("SlidingRefreshTokenLifetime")]
        public int SlidingRefreshTokenLifetime { get; set; }

        [JsonProperty("RefreshTokenUsage")]
        public long RefreshTokenUsage { get; set; }

        [JsonProperty("AccessTokenType")]
        public long AccessTokenType { get; set; }

        [JsonProperty("AllowOfflineAccess")]
        public bool AllowOfflineAccess { get; set; }

        [JsonProperty("RequireClientSecret")]
        public bool RequireClientSecret { get; set; }

        [JsonProperty("ClientClaimsPrefix")]
        public string ClientClaimsPrefix { get; set; }
    }

    public static class ClientRecordExtensions
    {
        public static Client ToClient(this ClientRecord self)
        {
            List<Secret> secrets = new List<Secret>();
            foreach (var secret in self.Secrets)
            {
                secrets.Add(new Secret(secret.Sha256()));
            }

            return new Client()
            {
                ClientId = self.ClientId,
                AllowedGrantTypes = self.AllowedGrantTypes,
                AllowOfflineAccess = self.AllowOfflineAccess,
                RefreshTokenUsage = (TokenUsage) self.RefreshTokenUsage,
                ClientSecrets = secrets,
                AllowedScopes = self.AllowedScopes,
                RequireClientSecret = self.RequireClientSecret,
                AccessTokenLifetime = self.AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = self.AbsoluteRefreshTokenLifetime,
                ClientClaimsPrefix = self.ClientClaimsPrefix
            };
        }

        public static List<Client> ToClients(this Dictionary<string, ClientRecord> self)
        {
            List<Client> result = new List<Client>();
            foreach (var clientRecord in self)
            {
                clientRecord.Value.ClientId = clientRecord.Key;
                var client = clientRecord.Value.ToClient();
                result.Add(client);
            }

            return result;
        }
    }

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
            IConfigurationSection section = Configuration.GetSection("clients");
            var clientRecords = new Dictionary<string, ClientRecord>();
            section.Bind(clientRecords);
            foreach (var clientRecord in clientRecords)
            {
                clientRecord.Value.ClientId = clientRecord.Key;
            }
            var clients = clientRecords.ToClients();

            section = Configuration.GetSection("apiResources");
            var apiResourceSettings = new List<string>();
            section.Bind(apiResourceSettings);
            List<ApiResource> apiResources = new List<ApiResource>();
            foreach (var apiResourceSetting in apiResourceSettings)
            {
                apiResources.Add(new ApiResource(apiResourceSetting));
            }

            services.AddSingleton<OIDCDiscoverCacheContainer>();

            var builder = services
                .AddIdentityServer(options => { options.InputLengthRestrictions.RefreshToken = 2048; })
                .AddDeveloperSigningCredential()
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
            // my replacement services.

            builder.AddRefreshTokenRevokationGeneratorWorkAround();

            services.AddArbitraryNoSubjectExtentionGrantTypes();
            services.AddArbitraryResourceOwnerExtentionGrantTypes();
            services.AddArbitraryOpenIdConnectTokenExtensionGrantTypes();
            services.AddIdentityServer4ExtraTypes();
            services.AddRefreshTokenRevokationGeneratorWorkAroundTypes();

            services.AddMemoryCache();
            services.AddMvc();

            services.AddLogging();
            services.AddWebEncoders();
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
