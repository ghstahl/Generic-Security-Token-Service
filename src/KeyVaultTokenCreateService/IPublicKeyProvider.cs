using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel.Jwk;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

namespace P7IdentityServer4
{
    public interface IPublicKeyProvider
    {
        Task<JsonWebKey> GetAsync();
        Task<KeyBundle> GetKeyBundleAsync();
        Task<KeyIdentifier> GetKeyIdentifierAsync();
    }
}