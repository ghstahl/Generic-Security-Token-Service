using System.Linq;
using IdentityServer4.Models;

namespace IdentityServer4Extras.Models
{
    internal static class ClientExtensions
    {
        public static bool IsImplicitOnly(this Client client)
        {
            return client != null &&
                   client.AllowedGrantTypes != null &&
                   client.AllowedGrantTypes.Count == 1 &&
                   client.AllowedGrantTypes.First() == GrantType.Implicit;
        }
    }
}