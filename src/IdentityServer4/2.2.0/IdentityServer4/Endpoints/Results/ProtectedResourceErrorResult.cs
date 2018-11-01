// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Primitives;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Endpoints.Results
{
    internal class ProtectedResourceErrorResult : IEndpointResult
    {
        public string Error;
        public string ErrorDescription;

        public ProtectedResourceErrorResult(string error, string errorDescription = null)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        public Task ExecuteAsync(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.SetNoCache();

            if (Constants.ProtectedResourceErrorStatusCodes.ContainsKey(Error))
            {
                context.Response.StatusCode = Constants.ProtectedResourceErrorStatusCodes[Error];
            }

            var errorString = string.Format($"error=\"{Error}\"");
            if (ErrorDescription.IsMissing())
            {
                context.Response.Headers.Add("WwwAuthentication", new StringValues(new[] { "Bearer", errorString }));
            }
            else
            {
                var errorDescriptionString = string.Format($"error_description=\"{ErrorDescription}\"");
                context.Response.Headers.Add("WwwAuthentication", new StringValues(new[] { "Bearer", errorString, errorDescriptionString }));
            }

            return Task.CompletedTask;
        }
 
        public Task ExecuteAsync(HttpResponseMessage httpResponseMessage)
        {
            var headers = httpResponseMessage.Headers;
            httpResponseMessage.StatusCode = HttpStatusCode.Unauthorized;
            headers.SetNoCache();

            if (Constants.ProtectedResourceErrorStatusCodes.ContainsKey(Error))
            {
                httpResponseMessage.StatusCode = (HttpStatusCode)Constants.ProtectedResourceErrorStatusCodes[Error];
            }

            var errorString = string.Format($"error=\"{Error}\"");
        
            if (ErrorDescription.IsMissing())
            {
                foreach (var sv in new StringValues(new[] { "Bearer", errorString }))
                {
                    headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(sv));
                }
            }
            else
            {
                var errorDescriptionString = string.Format($"error_description=\"{ErrorDescription}\"");
                foreach (var sv in new StringValues(new[] { "Bearer", errorString, errorDescriptionString }))
                {
                    headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(sv));
                }
            }

            return Task.CompletedTask;
        }
    }
}