using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ProfileServiceManager.Extensions
{
    public static class IdentityServer4Extensions
    {
        public static IIdentityServerBuilder AddProfileServiceManager(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IProfileService>();
            builder
                .AddProfileService<ProfileServiceManager>();
            builder.Services.AddTransient<IProfileServicePlugin, DefaultProfileServicePlugin>();
            return builder;
        }
    }
}
