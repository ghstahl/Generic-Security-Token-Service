using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentityClient.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AspNetCoreIdentityClient.Pages.Account
{
    public class SignoutFrontChannelModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        public SignoutFrontChannelModel(SignInManager<ApplicationUser> signInManager, ILogger<SignoutFrontChannelModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out via signout-frontchannel.");
        }
    }
}