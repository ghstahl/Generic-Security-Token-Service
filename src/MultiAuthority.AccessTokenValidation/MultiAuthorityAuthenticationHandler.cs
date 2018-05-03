// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MultiAuthority.AccessTokenValidation
{
    /// <summary>
    /// Authentication handler for validating both JWT and reference tokens
    /// </summary>
    public class MultiAuthorityAuthenticationHandler : AuthenticationHandler<MultiAuthorityAuthenticationOptions>
    {
        private readonly ILogger _logger;

        /// <inheritdoc />
        public MultiAuthorityAuthenticationHandler(
            IOptionsMonitor<MultiAuthorityAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<MultiAuthorityAuthenticationHandler>();
        }

        /// <summary>
        /// Tries to validate a token on the current request
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _logger.LogTrace("HandleAuthenticateAsync called");

            if (!Context.Request.Headers.ContainsKey("x-authScheme"))
            {
                return AuthenticateResult.NoResult();
            }

            var authScheme = Context.Request.Headers["x-authScheme"];
            if (string.IsNullOrWhiteSpace(authScheme))
            {
                return AuthenticateResult.NoResult();
            }

            if (!Global.SchemeRecords.ContainsKey(authScheme))
            {
                return AuthenticateResult.NoResult();
            }

            var jwtScheme = Scheme.Name + authScheme;

       
            var token = Options.TokenRetriever(Context.Request);
            bool removeToken = false;

            try
            {
                if (token != null)
                {
                    _logger.LogTrace("Token found: {token}", token);

                    removeToken = true;
                    Context.Items.Add(MultiAuthorityAuthenticationDefaults.TokenItemsKey, token);

                    // seems to be a JWT
                    if (token.Contains('.') && Options.SupportsJwt)
                    {
                        _logger.LogTrace("Token is a JWT and is supported.");

                        
                        Context.Items.Add(MultiAuthorityAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name, jwtScheme);
                        return await Context.AuthenticateAsync(jwtScheme);
                    }
                    
                    else
                    {
                        _logger.LogTrace("JWT token seem not to be correctly configured for incoming token.");
                    }
                }

                // set the default challenge handler to JwtBearer if supported
                if (Options.SupportsJwt)
                {
                    Context.Items.Add(MultiAuthorityAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name, jwtScheme);
                }

                return AuthenticateResult.NoResult();
            }
            finally
            {
                if (removeToken)
                {
                    Context.Items.Remove(MultiAuthorityAuthenticationDefaults.TokenItemsKey);
                }
            }
        }

        /// <summary>
        /// Override this method to deal with 401 challenge concerns, if an authentication scheme in question
        /// deals an authentication interaction as part of it's request flow. (like adding a response header, or
        /// changing the 401 result to 302 of a login page or external sign-in location.)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (Context.Items.TryGetValue(MultiAuthorityAuthenticationDefaults.EffectiveSchemeKey + Scheme.Name, out object value))
            {
                if (value is string scheme)
                {
                    _logger.LogTrace("Forwarding challenge to scheme: {scheme}", scheme);
                    await Context.ChallengeAsync(scheme);
                }
            }
            else
            {
                await base.HandleChallengeAsync(properties);
            }
        }
    }
}