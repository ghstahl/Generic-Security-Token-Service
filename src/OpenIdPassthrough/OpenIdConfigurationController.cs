using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OpenIdPassthrough.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using OpenIdPassthrough.Results;

namespace OpenIdPassthrough
{

    [Route("oidc")]
    public class OpenIdConfigurationController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public OpenIdConfigurationController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        [HttpGet]
        [Route(".well-known/openid-configuration")]
        public OpenIdConfiguration WellKnownOpenIdConfiguration()
        {
            var root = _hostingEnvironment.WebRootPath;
            var domain = $"{Request.Scheme}://{Request.Host}";
            return new OpenIdConfiguration()
            {
                Issuer = $"{domain}/oidc",
                TokenEndpoint = $"{domain}/oidc/connect/token",
                ScopesSupported = new []{ "offline_access" },
                GrantTypesSupported = new []{"refresh_token"},
                ResponseTypesSupported = new []{"token"},
                ResponseModesSupported = new []{ "form_post" }
            };
        }

        [HttpPost]
        [Route("connect/token")]
        public async Task TokenPassThrough()
        {
            var response = new OidcResponse();
            var form = (await Request.ReadFormAsync()).AsNameValueCollection();

            var id = form.Get("client_id");
            var refreshToken = form.Get("refresh_token");
            var grantType = form.Get("grant_type");


            OpenIdPassthrough.Results.BadRequestResult badResult = new OpenIdPassthrough.Results.BadRequestResult("invalid request");
            await badResult.ExecuteAsync(HttpContext);

        }
    }
}
