using IdentityServer4.Stores;
using IdentityServer4Extras.Endpoints;
using IdentityServer4Extras.Services;
using IdentityServer4Extras.Stores;
using IdentityServer4Extras.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4Extras.Extensions
{
    public static class AspNetCoreServiceExtensions
    {
        public static void AddIdentityServer4ExtraTypes(this IServiceCollection services)
        {
            services.AddTransient<IRawClientSecretValidator, RawClientSecretValidator>();
            services.AddTransient<PrincipalAugmenter>();
            services.AddScoped<IEndpointHandlerExtra, TokenEndpointExtra>();
            services.AddTransient<ISecretParserExtra, PostBodySecretParserExtra>();
            services.TryAddSingleton<ITokenRevocationEventHandler, NullTokenRevocationEventHandler>();
        }
    }
}