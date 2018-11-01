using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4Extras.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4Extras.Endpoints
{
    public class TokenErrorResult : IEndpointResult
    {
        public TokenErrorResponse Response { get; }

        public TokenErrorResult(TokenErrorResponse error)
        {
            if (error.Error.IsMissing()) throw new ArgumentNullException("Error must be set", nameof(error.Error));

            Response = error;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                error = Response.Error,
                error_description = Response.ErrorDescription
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
            public string error { get; set; }
            public string error_description { get; set; }
        }

        public async Task<ActionResult> BuildActionResultAsync()
        {
            var expando = new ExpandoObject();
            dynamic expandoDynamic = expando as dynamic;
            expandoDynamic.error = Response.Error;
            expandoDynamic.error_description = Response.ErrorDescription;

            
            if (!Response.Custom.IsNullOrEmpty())
            {
                IDictionary<string, object> dictionary_object = expando;
                dictionary_object.AddDictionary(Response.Custom);
            }
            var inner = new JsonResult(expandoDynamic);

            var result = new CustomActionResult<JsonResult>(inner)
            {
                SetNoCache = true
            };
            return result;
        }
    }
}