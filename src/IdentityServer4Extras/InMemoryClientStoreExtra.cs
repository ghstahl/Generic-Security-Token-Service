using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4Extras
{
    /// <summary>
    /// In-memory client store
    /// </summary>
    public class InMemoryClientStoreExtra : InMemoryClientStore, IClientStoreExtra
    {
        private readonly IEnumerable<Client> _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServer4.Stores.InMemoryClientStore"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public InMemoryClientStoreExtra(IEnumerable<Client> clients):base(clients)
        {}


        public async Task<ClientExtra> FindClientExtraByIdAsync(string clientId)
        {
            var client = await FindClientByIdAsync(clientId);
            return client as ClientExtra;
        }
    }
}