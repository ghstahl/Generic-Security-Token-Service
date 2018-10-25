using System.Threading.Tasks;

namespace IdentityServer4Extras.Services
{
    public interface ITokenRevocationEventHandler
    {
        Task TokenRevokedAsync(ClientExtra clientExtra, string subjectId);
    }
}