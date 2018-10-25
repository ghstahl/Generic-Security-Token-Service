using System.Threading.Tasks;

namespace IdentityServer4Extras.Services
{
    public class NullTokenRevocationEventHandler : ITokenRevocationEventHandler
    {
        public Task TokenRevokedAsync(ClientExtra clientExtra, string subjectId)
        {
            return Task.CompletedTask;
        }
    }
}