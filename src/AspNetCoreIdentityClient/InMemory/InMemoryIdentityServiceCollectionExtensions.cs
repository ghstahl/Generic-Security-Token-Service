using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AspNetCoreIdentityClient.InMemory
{
    public static class InMemoryIdentityServiceCollectionExtensions
    {
        public static IdentityBuilder AddAuthentication<TUser>(this IServiceCollection services, IConfiguration configuration)
            where TUser : class => services.AddAuthentication<TUser>(configuration, null);

        public static IdentityBuilder AddAuthentication<TUser>(this IServiceCollection services, IConfiguration configuration, Action<IdentityOptions> setupAction)
            where TUser : class
        {
            // Services used by identity
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });
 

            authenticationBuilder.AddOpenIdConnect("oidc", options =>
            {
             //   options.SignInScheme = "Cookies";

                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "mvc2";
                options.SaveTokens = true;
                options.Events.OnRedirectToIdentityProvider = async n =>
                {
                    if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                    {
                        n.ProtocolMessage.AcrValues = "idp=google";
                    }
                };
            });
         
            return new IdentityBuilder(typeof(TUser), services);
        }
    }
}