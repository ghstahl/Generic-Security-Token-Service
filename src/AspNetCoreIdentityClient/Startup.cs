using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreIdentityClient.Data;
using AspNetCoreIdentityClient.InMemory;
using AspNetCoreIdentityClient.Services;
using IdentityModelExtras;
using Microsoft.Extensions.DependencyInjection.Extensions;
using P7.AspNetCore.Identity.InMemory;

namespace AspNetCoreIdentityClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Name = $"{Configuration["applicationName"]}.AspNetCore.Consent";
            });
            services.AddSingleton<ConfiguredDiscoverCacheContainerFactory>();

            var inMemoryStore = new InMemoryStore<ApplicationUser, ApplicationRole>();

            services.AddSingleton<IUserStore<ApplicationUser>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddSingleton<IUserRoleStore<ApplicationUser>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddSingleton<IRoleStore<ApplicationRole>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = $"{Configuration["applicationName"]}.AspNetCore.Identity.Application";
            });
            services.AddAuthentication<ApplicationUser>(Configuration);
            services
                .AddScoped
                <Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<ApplicationUser>,
                    AppClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();
            
            // Hosting doesn't add IHttpContextAccessor by default
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
