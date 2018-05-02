// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace MultiAuthority.AccessTokenValidation
{
    internal class ConfigureInternalOptions : 
        IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly MultiAuthorityAuthenticationOptions _multiAuthorityOptions;
        private string _scheme;

        public ConfigureInternalOptions(MultiAuthorityAuthenticationOptions multiAuthorityOptions, string scheme)
        {
            _multiAuthorityOptions = multiAuthorityOptions;
            _scheme = scheme;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == _scheme + MultiAuthorityAuthenticationDefaults.JwtAuthenticationScheme &&
                _multiAuthorityOptions.SupportsJwt)
            {
                _multiAuthorityOptions.ConfigureJwtBearer(options);
            }
        }
        public void Configure(JwtBearerOptions options)
        { }

        public void Configure(OAuth2IntrospectionOptions options)
        { }
    }
}