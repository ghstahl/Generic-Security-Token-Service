using System;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace IdentityServer4Extras.Stores
{
    public interface IPersistedGrantStore2 : IPersistedGrantStore
    {
        //
        // Summary:
        //     Removes all grants for a given subject id.
        //
        // Parameters:
        //   subjectId:
        //     The subject identifier.
        //
        //   clientId:
        //     The client identifier.
        Task RemoveAllAsync(string subjectId);
    }
}
