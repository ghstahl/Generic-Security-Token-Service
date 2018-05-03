using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiAuthority.AccessTokenValidation;

namespace apiHost
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
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

 
            List<SchemeRecord> schemeRecords = new List<SchemeRecord>()
            {
                new SchemeRecord()
                {
                    Name = "One",
                    JwtBearerOptions = options =>
                    {
                        options.Authority = "https://p7identityserver4.azurewebsites.net";
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudiences = new List<string>()
                            {
                                "nitro"
                            } 
                        };
                    }
                },new SchemeRecord()
                {
                    Name = "Two",
                    JwtBearerOptions = options =>
                    {
                        options.Authority = "https://p7identityserver4two.azurewebsites.net";
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudiences = new List<string>()
                            {
                                "metal"
                            }
                        };
                    }
                }
            };

 
            services.AddAuthentication("Bearer")
                .AddMultiAuthorityAuthentication(schemeRecords, options =>
                {
 
                });
            /*
                .AddIdentityServerAuthentication(options =>
                {
                    // https://p7identityserver4.azurewebsites.net
                    options.Authority = "http://localhost:21354";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "nitro";
                });
                */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
