// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Endpoints.Results
{
    internal class TokenResult : IEndpointResult
    {
        public TokenResponse Response { get; set; }

        public TokenResult(TokenResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            Response = response;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                id_token = Response.IdentityToken,
                access_token = Response.AccessToken,
                refresh_token = Response.RefreshToken,
                expires_in = Response.AccessTokenLifetime,
                token_type = OidcConstants.TokenResponse.BearerTokenType
            };

            if (Response.Custom.IsNullOrEmpty())
            {
                await context.Response.WriteJsonAsync(dto);
            }
            else
            {
                var jobject = ObjectSerializer.ToJObject(dto);
                jobject.AddDictionary(Response.Custom);

                await context.Response.WriteJsonAsync(jobject);
            }
        }

        internal class ResultDto
        {
            public string id_token { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
        }
 
        public async Task ExecuteAsync(HttpResponseMessage httpResponseMessage)
        {
            var headers = httpResponseMessage.Headers;
            headers.SetNoCache();

            var expando = new ExpandoObject();
            dynamic expandoDynamic = expando as dynamic;
            expandoDynamic.access_token = Response.AccessToken;
            if (!string.IsNullOrEmpty(Response.IdentityToken))
            {
                expandoDynamic.id_token = Response.IdentityToken;
            }
            if (!string.IsNullOrEmpty(Response.RefreshToken))
            {
                expandoDynamic.refresh_token = Response.RefreshToken;
            }
            expandoDynamic.expires_in = Response.AccessTokenLifetime;
            expandoDynamic.token_type = OidcConstants.TokenResponse.BearerTokenType;

            if (!Response.Custom.IsNullOrEmpty())
            {
                IDictionary<string, object> dictionary_object = expando;
                dictionary_object.AddDictionary(Response.Custom);
            }
            httpResponseMessage.Content = new JsonContent(expandoDynamic);
        }
    }
}