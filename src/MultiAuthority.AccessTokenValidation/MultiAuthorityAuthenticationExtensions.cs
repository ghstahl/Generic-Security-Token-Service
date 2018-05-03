// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultiAuthority.AccessTokenValidation;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extensions for registering the MultiAuthority authentication handler
    /// </summary>
    public static class MultiAuthorityAuthenticationExtensions
    {
        /// <summary>
        /// Registers the MultiAuthority authentication handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddMultiAuthorityAuthentication(this AuthenticationBuilder builder, 
            string authenticationScheme, 
            IEnumerable<SchemeRecord> schemeRecords, Action<MultiAuthorityAuthenticationOptions> configureOptions)
        {
            foreach (var schemeRecord in schemeRecords)
            {
                builder.AddJwtBearer(authenticationScheme + schemeRecord.Name, configureOptions: schemeRecord.Options);
            }

            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(services =>
            {
                var monitor = services.GetRequiredService<IOptionsMonitor<MultiAuthorityAuthenticationOptions>>();
                return new ConfigureInternalOptions(monitor.Get(authenticationScheme), authenticationScheme);
            });

            return builder.AddScheme<MultiAuthorityAuthenticationOptions, MultiAuthorityAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}