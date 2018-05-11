using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Endpoints.Results
{
    internal class Authorize2Result : IEndpointResult
    {
        public AuthorizeResponse Response { get; }

        public Authorize2Result(AuthorizeResult authorizeResult)
        {
            Response = authorizeResult.Response ?? throw new ArgumentNullException(nameof(authorizeResult));
        }

        private IdentityServerOptions _options;
        
        private IMessageStore<ErrorMessage> _errorMessageStore;
        private ISystemClock _clock;

        private void Init(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _errorMessageStore = _errorMessageStore ?? context.RequestServices.GetRequiredService<IMessageStore<ErrorMessage>>();
            _clock = _clock ?? context.RequestServices.GetRequiredService<ISystemClock>();
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            if (Response.IsError)
            {
                await ProcessErrorAsync(context);
            }
            else
            {
                await ProcessResponseAsync(context);
            }
        }

        private async Task ProcessErrorAsync(HttpContext context)
        {
            // these are the conditions where we can send a response 
            // back directly to the client, otherwise we're only showing the error UI
            var isPromptNoneError = Response.Error == OidcConstants.AuthorizeErrors.AccountSelectionRequired ||
                                    Response.Error == OidcConstants.AuthorizeErrors.LoginRequired ||
                                    Response.Error == OidcConstants.AuthorizeErrors.ConsentRequired ||
                                    Response.Error == OidcConstants.AuthorizeErrors.InteractionRequired;

            // this scenario we can return back to the client
            await ProcessResponseAsync(context);

        }

        protected async Task ProcessResponseAsync(HttpContext context)
        {
            await RenderAuthorizeResponseAsync(context);
        }

        private async Task RenderAuthorizeResponseAsync(HttpContext context)
        {
            var dto = new ResultDto
            {
                code = Response.Code,
                expires_in = Response.AccessTokenLifetime,
            };
            context.Response.SetNoCache();
            await context.Response.WriteJsonAsync(dto);

        }

        private void AddSecurityHeaders(HttpContext context)
        {
            context.Response.AddScriptCspHeaders(_options.Csp, "sha256-VuNUSJ59bpCpw62HM2JG/hCyGiqoPN3NqGvNXQPU+rY=");

            var referrer_policy = "no-referrer";
            if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
            {
                context.Response.Headers.Add("Referrer-Policy", referrer_policy);
            }
        }

        internal class ResultDto
        {
            public string code { get; set; }
            public int expires_in { get; set; }
        }
    }
}