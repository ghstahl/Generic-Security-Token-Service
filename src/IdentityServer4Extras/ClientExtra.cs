using IdentityServer4.Models;

namespace IdentityServer4Extras
{
    public class ClientExtra: Client
    {
        /// <summary>
        /// If set to false, no client secret is needed to request tokens at the token endpoint (defaults to <c>true</c>)
        /// </summary>
        public bool RequireRefreshTokenClientSecret { get; set; } = true;
    }
}