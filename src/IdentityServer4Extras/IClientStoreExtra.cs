using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServer4Extras
{
    public interface IClientStoreExtra
    {
        Task<ClientExtra> FindClientExtraByIdAsync(string clientId);
    }
}