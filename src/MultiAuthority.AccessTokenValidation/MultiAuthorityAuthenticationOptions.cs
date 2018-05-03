// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MultiAuthority.AccessTokenValidation
{
    /// <summary>
    /// Options for IdentityServer authentication
    /// </summary>
    public class MultiAuthorityAuthenticationOptions : AuthenticationSchemeOptions
    {
        static readonly Func<HttpRequest, string> InternalTokenRetriever = request => request.HttpContext.Items[MultiAuthorityAuthenticationDefaults.TokenItemsKey] as string;
        
        /// <summary>
        /// Base-address of the token issuer
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Specifies whether HTTPS is required for the discovery endpoint
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        /// Specifies which token types are supported (JWT, reference or both)
        /// </summary>
        public SupportedTokens SupportedTokens { get; set; } = SupportedTokens.Both;

        /// <summary>
        /// Callback to retrieve token from incoming request
        /// </summary>
        public Func<HttpRequest, string> TokenRetriever { get; set; } = TokenRetrieval.FromAuthorizationHeader();
 
        /// <summary>
        /// Claim type for name
        /// </summary>
        public string NameClaimType { get; set; } = "name";

        /// <summary>
        /// Claim type for role
        /// </summary>
        public string RoleClaimType { get; set; } = "role";

        /// <summary>
        /// Specifies inbound claim type map for JWT tokens (mainly used to disable the annoying default behavior of the MS JWT handler)
        /// </summary>
        public Dictionary<string, string> InboundJwtClaimTypeMap { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// specifies whether the token should be saved in the authentication properties
        /// </summary>
        public bool SaveToken { get; set; } = true;

        /// <summary>
        /// specifies the allowed clock skew when validating JWT tokens
        /// </summary>
        public TimeSpan? JwtValidationClockSkew { get; set; }

   
        /// <summary>
        /// timeout for back-channel operations
        /// </summary>
        public TimeSpan BackChannelTimeouts { get; set; } = TimeSpan.FromSeconds(60);

        // todo
        /// <summary>
        /// events for JWT middleware
        /// </summary>
        public JwtBearerEvents JwtBearerEvents { get; set; } = new JwtBearerEvents();

        /// <summary>
        /// Specifies how often the cached copy of the discovery document should be refreshed.
        /// If not set, it defaults to the default value of Microsoft's underlying configuration manager (which right now is 24h).
        /// If you need more fine grained control, provide your own configuration manager on the JWT options.
        /// </summary>
        public TimeSpan? DiscoveryDocumentRefreshInterval { get; set; }

        /// <summary>
        /// Gets a value indicating whether JWTs are supported.
        /// </summary>
        public bool SupportsJwt => SupportedTokens == SupportedTokens.Jwt || SupportedTokens == SupportedTokens.Both;
 
    }
}