// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Hosting
{
    /// <summary>
    /// Endpoint result
    /// </summary>
    public interface IEndpointResult2
    {
        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="httpResponseMessage">The HttpResponseMessage.</param>
        /// <returns></returns>
        Task ExecuteAsync(HttpResponseMessage httpResponseMessage);
    }
    /// <summary>
    /// Endpoint result
    /// </summary>
    public interface IEndpointResult: IEndpointResult2
    {
        /// <summary>
        /// Executes the result.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        Task ExecuteAsync(HttpContext context);
    }
    
}