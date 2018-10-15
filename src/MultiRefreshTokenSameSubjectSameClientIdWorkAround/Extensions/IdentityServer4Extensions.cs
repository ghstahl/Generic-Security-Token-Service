using IdentityServer4.ResponseHandling;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MultiRefreshTokenSameSubjectSameClientIdWorkAround.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddRefreshTokenRevokationGeneratorWorkAround(this IIdentityServerBuilder builder)
        {
            builder.Services.Remove<IRefreshTokenStore>();
            builder.Services.AddTransient<DefaultRefreshTokenStoreEnhancement>();
            builder.Services.TryAddTransient<IRefreshTokenStore, ProxyDefaultRefreshTokenStoreEnhancement>();

            builder.Services.Remove<ITokenRevocationResponseGenerator>();
            builder.Services.TryAddTransient<ITokenRevocationResponseGenerator, MyTokenRevocationResponseGenerator>();
            return builder;
        }
    }
}