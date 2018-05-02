// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MultiAuthority.AccessTokenValidation
{
    /// <summary>
    /// Constants for MultiAuthority authentication.
    /// </summary>
    public class MultiAuthorityAuthenticationDefaults
    {
        /// <summary>
        /// The authentication scheme
        /// </summary>
        public const string AuthenticationScheme = "Bearer";

        internal const string JwtAuthenticationScheme = "MultiAuthorityAuthenticationJwt";
        internal const string TokenItemsKey = "ma:tokenvalidation:token";
        internal const string EffectiveSchemeKey = "ma:tokenvalidation:effective:";
    }
}