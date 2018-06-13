using System;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using ProfileServiceManager;

namespace IdentityServer4.AspNetIdentityExtras
{
    public class AspNetIdentityProfileServiceExtra<TUser> : ProfileService<TUser>, IProfileServicePlugin where TUser : class
    {
        private ProfileService<TUser> _profileServiceImplementation;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        public AspNetIdentityProfileServiceExtra(UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory):base(userManager, claimsFactory)
        {
            
        }
        public string Name => "default";
        public IProfileService ProfileService => this;
    }
}
