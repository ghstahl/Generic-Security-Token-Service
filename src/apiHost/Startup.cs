using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using apiHost.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiAuthority.AccessTokenValidation;

namespace apiHost
{
    internal static class Global
    {
        public static string SecurityKey =
            "OfED+KgbZxtu4e4+JSQWdtSgTnuNixKy1nMVAEww8QL3IN3idcNgbNDSSaV4491Fo3sq2aGSCtYvekzs7JwXJnNAyvDSJjfK/7M8MpxSMnm1vMscBXyiYFXhGC4wqWlYBE828/5DNyw3QZW5EjD7hvDrY5OlYd4smCTa53helNnJz5NT9HQaDbE2sMwIDAQABAoIBAEs63TvT94njrPDP3A/sfCEXg1F2y0D/PjzUhM1aJGcRiOUXnGlYdViGhLnnJoNZTZm9qI1LT0NWcDA5NmBN6gcrk2EApyTt1D1i4AQ66rYoTF9iEC4Wye28v245BYESA6IIelgIxXGsVyllERsbTkaphzibbYfHmvwMxkn135Zfzd/NOXl/O32vYIomzrNEP+tN2WXhhG8c8+iZ8PErBV3CqrYogYy97d2CeQbXcpd5unPiU4TK0nnzeBAXdgeYuJHFC45YHl9UvShRoe6CHR47ceIGp6WMc5BTyyTkZpctuYJTwaChdj/QuRSkTYmn6jFL+MRfYQJ8VVwSVo5DbkECgYEA4/YIMKcwObYcSuHzgkMwH645CRDoy9M98eptAoNLdJBHYz23U5IbGL1+qHDDCPXxKs9ZG7EEqyWezq42eoFoebLA5O6/xrYXoaeIb094dbCF4D932hAkgAaAZkZVsSiWDCjYSV+JoWX4NVBcIL9yyHRhaaPVULTRbPsZQWq9+hMCgYEA48j4RGO7CaVpgUVobYasJnkGSdhkSCd1VwgvHH3vtuk7/JGUBRaZc0WZGcXkAJXnLh7QnDHOzWASdaxVgnuviaDi4CIkmTCfRqPesgDR2Iu35iQsH7P2/o1pzhpXQS/Ct6J7/GwJTqcXCvp4tfZDbFxS8oewzp4RstILj+pDyWECgYByQAbOy5xB8GGxrhjrOl1OI3V2c8EZFqA/NKy5y6/vlbgRpwbQnbNy7NYj+Y/mV80tFYqldEzQsiQrlei78Uu5YruGgZogL3ccj+izUPMgmP4f6+9XnSuN9rQ3jhy4k4zQP1BXRcim2YJSxhnGV+1hReLknTX2IwmrQxXfUW4xfQKBgAHZW8qSVK5bXWPjQFnDQhp92QM4cnfzegxe0KMWkp+VfRsrw1vXNx";

    }

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
                .AddAuthorization(options =>
                {
                    options.AddPolicy("aggregator_service",
                        policy => { policy.RequireClaim("nag_aud", "aggregator_service"); });
                    options.AddPolicy("aggregator_service.read_only",
                        policy => { policy.RequireClaim("nag_scope", "aggregator_service.read_only"); });
                    options.AddPolicy("aggregator_service.full_access",
                        policy => { policy.RequireClaim("nag_scope", "aggregator_service.full_access"); });
                })
                .AddJsonFormatters();

            var securityKey = Global.SecurityKey;
            var securityKeyBytes = Encoding.UTF8.GetBytes(securityKey);
            var issuerSigningKey = new SymmetricSecurityKey(securityKeyBytes);

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
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudiences = new List<string>()
                            {
                                "aggregator_service"
                            }
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

                                    // get rid of the namespace on the claims.
                                    var newClaims = new List<Claim>();
                                    foreach (var claim in identity.Claims)
                                    {
                                        var claimType = claim.Type;
                                        if (claimType.StartsWith("blah."))
                                        {
                                            claimType = claimType.Substring("blah.".Length);
                                            newClaims.Add(new Claim(claimType, claim.Value));
                                        }
                                        else
                                        {
                                            newClaims.Add(claim);
                                        }
                                    }

                                    // replace the entire principal
                                    var appIdentity = new ClaimsIdentity(newClaims);
                                    var claimsPrincipal = new ClaimsPrincipal(appIdentity);
                                    context.Principal = claimsPrincipal;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    }

                },
                new SchemeRecord()
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
                },
                new SchemeRecord()
                {
                    Name = "local",
                    JwtBearerOptions = options =>
                    {
                        options.Authority = "https://localhost:44332/";
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudiences = new List<string>()
                            {
                                "metal",
                                "nag",
                                "olp"
                            }
                        };
                    }
                },
                new SchemeRecord()
                {
                    Name = "Self",
                    JwtBearerOptions = options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = "yourdomain.com",
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudiences = new List<string>()
                            {
                                "nitro"
                            },
                            IssuerSigningKey = issuerSigningKey
                        };
                    }
                }
            };

            services.AddAuthentication("Bearer")
                .AddMultiAuthorityAuthentication(schemeRecords);

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
                        where claim.Type == "aud" && claim.Value == "aggregator_service"
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
            app.UseMvc();
        }
    }
}
