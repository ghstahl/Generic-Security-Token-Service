using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityServer4Extras.Services;
using IdentityServer4Extras.Stores;
using IdentityServer4Extras.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4Extras.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddPluginHostClientSecretValidator(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ClientSecretValidator>();
            builder.Services.RemoveAll<IClientSecretValidator>();
            builder.Services.AddTransient<ClientSecretValidator>();
            builder.Services.TryAddTransient<IClientSecretValidator, PluginHostClientSecretValidator>();
            return builder;
        }
        public static IIdentityServerBuilder AddNoSecretRefreshClientSecretValidator(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientSecretValidatorPlugin, NoSecretRefreshClientSecretValidator>();
            return builder;
        }
       
        public static IIdentityServerBuilder AddInMemoryClientStoreExtra(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IClientStore>();
            builder.Services.TryAddSingleton<IClientStoreExtra, InMemoryClientStoreExtra>();
            builder.Services.TryAddSingleton<IClientStore>(x => x.GetService<IClientStoreExtra>());
            return builder;
        }

        public static IIdentityServerBuilder AddNullRefreshTokenKeyObfuscator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IRefreshTokenKeyObfuscator>();
            builder.Services.AddTransient<IRefreshTokenKeyObfuscator, NullRefreshTokenKeyObfuscator>();
            return builder;
        }
        public static IIdentityServerBuilder AddProtectedRefreshTokenKeyObfuscator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IRefreshTokenKeyObfuscator>();
            builder.Services.AddTransient<IRefreshTokenKeyObfuscator, ProtectedRefreshTokenKeyObfuscator>();
            return builder;
        }

        public static IIdentityServerBuilder SwapOutTokenResponseGenerator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenResponseGenerator>();
            builder.Services.TryAddTransient<ITokenResponseGenerator, TokenResponseGeneratorHook>();
            return builder;
        }

        public static IIdentityServerBuilder SwapOutDefaultTokenService(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenService>();
            builder.Services.TryAddTransient<DefaultTokenService>();
            builder.Services.TryAddTransient<ITokenService, DefaultTokenServiceHook>();
            return builder;
        }

        public static IIdentityServerBuilder SwapOutScopeValidator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IScopeValidator>();
            builder.Services.TryAddTransient<IScopeValidator, MyScopeValidator>();
            return builder;
        }
        public static IIdentityServerBuilder SwapOutTokenRevocationRequestValidator(
            this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<ITokenRevocationRequestValidator>();
            builder.Services.TryAddTransient<ITokenRevocationRequestValidator, SubjectTokenRevocationRequestValidator>();
            return builder;
        }

    }
}
