using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Extras
{
    public static class IdentityServer4Extensions
    {
        /// <summary>
        /// Adds the in memory clients.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="clients">The clients.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddInMemoryClientsExtra(this IIdentityServerBuilder builder, 
            IEnumerable<Client> clients)
        {
            builder.Services.AddSingleton(clients);

            builder.AddClientStore<InMemoryClientStoreExtra>();

            var existingCors = builder.Services.Where(x => x.ServiceType == typeof(ICorsPolicyService)).LastOrDefault();
            if (existingCors != null &&
                existingCors.ImplementationType == typeof(DefaultCorsPolicyService) &&
                existingCors.Lifetime == ServiceLifetime.Transient)
            {
                // if our default is registered, then overwrite with the InMemoryCorsPolicyService
                // otherwise don't overwrite with the InMemoryCorsPolicyService, which uses the custom one registered by the host
                builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
            }
            return builder;
        }

        public static IIdentityServerBuilder AddIdentityServer4Extras(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<ISecretParser, ClientRefreshTokenRequiredSecretParser>();
            builder.Services.AddTransient<ISecretValidator, ClientRefreshTokenRequiredSecretValidator>();

            return builder;
        }
    }
}