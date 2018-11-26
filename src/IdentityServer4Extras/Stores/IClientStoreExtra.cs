using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace IdentityServer4Extras.Stores
{
    public interface IClientStoreExtra: IClientStore
    {
        //
        // Summary:
        //     gets all known clientIds
        //
        // Returns:
        //     The list of all clientIds
        Task<List<string>> GetAllClientIdsAsync();

        //
        // Summary:
        //     gets all known clients
        //
        // Returns:
        //     The list of all clientIds
        Task<List<ClientExtra>> GetAllClientsAsync();

        //
        // Summary:
        //     gets all known clientIds that are enabled
        //
        // Returns:
        //     The list of all clientIds
        Task<List<string>> GetAllEnabledClientIdsAsync();
    }
}