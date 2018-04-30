using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ProfileServiceManager.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddProfileServiceManager(this IIdentityServerBuilder builder)
        {
            builder
                .AddProfileService<ProfileServiceManager>();
            return builder;
        }
    }
}
