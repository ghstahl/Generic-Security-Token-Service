using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiAuthority.AccessTokenValidation;
using WizardAppApi.Services;

namespace WizardAppApi
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
            services.AddScoped<IJsonFileLoader, JsonFileLoader>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddAuthorization(options =>
                {
                    options.AddPolicy("Daffy Duck",
                        policy => { policy.RequireClaim("client_namespace", "Daffy Duck"); });
                })
                .AddJsonFormatters();
            List<SchemeRecord> schemeRecords = new List<SchemeRecord>()
            {  new SchemeRecord()
                {
                    Name = "One",
                    JwtBearerOptions = options =>
                    {
                        options.Authority = "https://p7identityserver4.azurewebsites.net";
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true 
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context => { return Task.CompletedTask; },
                            OnTokenValidated = context =>
                            {

                                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                                if (identity != null)
                                {
                                    // Add the access_token as a claim, as we may actually need it
                                    var accessToken = context.SecurityToken as JwtSecurityToken;
                                    if (accessToken != null)
                                    {
                                        if (identity != null)
                                        {
                                            identity.AddClaim(new Claim("access_token", accessToken.RawData));
                                        }
                                    }
                                }

                                return Task.CompletedTask;
                            }
                        };
                    }

                },
            };
            services.AddAuthentication("Bearer")
                .AddMultiAuthorityAuthentication(schemeRecords);
            services.AddHttpContextAccessor(); services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                //do work before the invoking the rest of the pipeline       
                if (context.Request.Headers.ContainsKey("x-authScheme") &&
                    context.Request.Headers.ContainsKey("Authorization") &&
                    context.User != null)
                {
                    // looking for bearer token stuff.
                    var claims = context.User.Claims;
                    var q = from claim in claims
                        where claim.Type == "aud" && claim.Value == "wizard"
                        select claim;
                    if (!q.Any())
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }

                await next.Invoke(); //let the rest of the pipeline run

                //do work after the rest of the pipeline has run     
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
