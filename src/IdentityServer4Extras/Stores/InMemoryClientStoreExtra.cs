using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4Extras.Stores
{
    //
    // Summary:
    //     Retrieval of client configuration
    /// <summary>
    /// In-memory client store
    /// </summary>
    public class InMemoryClientStoreExtra : IClientStoreExtra
    {
        private readonly IEnumerable<Client> _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryClientStore"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public InMemoryClientStoreExtra(IEnumerable<Client> clients)
        {
            if (clients.HasDuplicates(m => m.ClientId))
            {
                throw new ArgumentException("Clients must not contain duplicate ids");
            }

            _clients = clients;
        }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var query =
                from client in _clients
                where client.ClientId == clientId
                select client;

            return Task.FromResult(query.SingleOrDefault());
        }

        public Task<List<string>> GetAllClientIdsAsync()
        {
            var query =
                from client in _clients
                select client.ClientId;

            return Task.FromResult(query.ToList());
        }

        public Task<List<string>> GetAllEnabledClientIdsAsync()
        {
            var query =
                from client in _clients
                where client.Enabled == true
                select client.ClientId;

            return Task.FromResult(query.ToList());
        }
    }
}
