using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentityClient.Data;
using IdentityModelExtras;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AspNetCoreIdentityClient.Pages.Account
{
    public class FrontChannelLogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ConfiguredDiscoverCacheContainerFactory _configuredDiscoverCacheContainerFactory;
        private readonly ILogger _logger;
        public FrontChannelLogoutModel(
            SignInManager<ApplicationUser> signInManager,
            ConfiguredDiscoverCacheContainerFactory configuredDiscoverCacheContainerFactory,
            ILogger<FrontChannelLogoutModel> logger)
        {
            _signInManager = signInManager;
            _configuredDiscoverCacheContainerFactory = configuredDiscoverCacheContainerFactory;
            _logger = logger;
        }

        public string IdToken { get; private set; }
        public string EndSessionUrl { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var clientSignoutCallback = $"{Request.Scheme}://{Request.Host}/Account/SignoutCallbackOidc";
            // Get id_token first to seed the iframe logout
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user != null)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                var query = from item in info.AuthenticationTokens
                    where item.Name == "id_token"
                    select item;
                var idToken = query.FirstOrDefault();
                IdToken = idToken.Value;

                var discoverCacheContainer = _configuredDiscoverCacheContainerFactory.Get(info.LoginProvider);
                var discoveryCache = await discoverCacheContainer.DiscoveryCache.GetAsync();
                var endSession = discoveryCache.EndSessionEndpoint;
                EndSessionUrl = $"{endSession}?id_token_hint={IdToken}&post_logout_redirect_uri={clientSignoutCallback}";

                // no matter what, we are logging out our own app.
                // Do Not trust the provider to keep its end of the bargain to frontchannel sign us out.
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");

                // this redirect is to the provider to log everyone else out.  
                // We will get a double hit here, as our $"{Request.Scheme}://{Request.Host}/Account/SignoutFrontChannel";
                // will get hit as well.  
                return new RedirectResult(EndSessionUrl);
                //return Page();  return this is you want iFrame loggout.  Your OIDC provider needs to let this go through though.
            }
            return new RedirectResult("/");
        }

    }
}