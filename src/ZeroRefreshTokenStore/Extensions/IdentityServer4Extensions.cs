using IdentityServer4.ResponseHandling;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZeroRefreshTokenStore.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddZeroRefreshTokenStore(this IIdentityServerBuilder builder)
        {
            builder.Services.Remove<IRefreshTokenStore>();
            builder.Services.TryAddTransient<IRefreshTokenStore, MyZeroRefreshTokenStore>();
            return builder;
        }
    }
}