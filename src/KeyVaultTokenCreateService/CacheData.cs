using System.Collections.Generic;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Tokens;

namespace P7IdentityServer4
{
    public class CacheData
    {
        public List<RsaSecurityKey> RsaSecurityKeys { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public List<JsonWebKey> JsonWebKeys { get; set; }
        public KeyIdentifier KeyIdentifier { get; set; }
    }
}