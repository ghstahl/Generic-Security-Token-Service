// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.Core.Utils;

namespace IdentityServer4.Stores
{
    /// <summary>
    /// In-memory resource store
    /// </summary>
    public class InMemoryResourcesStore : IResourceStore
    {
        private readonly IEnumerable<IdentityResource> _identityResources;
        private readonly IEnumerable<ApiResource> _apiResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryResourcesStore" /> class.
        /// </summary>
        public InMemoryResourcesStore(IEnumerable<IdentityResource> identityResources = null, IEnumerable<ApiResource> apiResources = null)
        {
            if (identityResources?.HasDuplicates(m => m.Name) == true)
            {
                throw new ArgumentException("Identity resources must not contain duplicate names");
            }

            if (apiResources?.HasDuplicates(m => m.Name) == true)
            {
                throw new ArgumentException("Api resources must not contain duplicate names");
            }

            _identityResources = identityResources ?? Enumerable.Empty<IdentityResource>();
            _apiResources = apiResources ?? Enumerable.Empty<ApiResource>();
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(_identityResources, _apiResources);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            Guard.ArgumentNotNull(nameof(name), name);
            var api = from a in _apiResources
                      where a.Name == name
                      select a;
            return Task.FromResult(api.FirstOrDefault());
        }

        /// <summary>
        /// Finds the identity resources by scope.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">names</exception>
        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> names)
        {
            Guard.ArgumentNotNull(nameof(names), names);

            var identity = from i in _identityResources
                           where names.Contains(i.Name)
                           select i;

            return Task.FromResult(identity);
        }

        /// <summary>
        /// Finds the API resources by scope.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">names</exception>
        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> names)
        {
            Guard.ArgumentNotNull(nameof(names), names);
            /*
            var api = from a in _apiResources
                      let scopes = (from s in a.Scopes where names.Contains(s.Name) select s)
                      where scopes.Any()
                      select a;
            */
            var api = from name in names
                let c = new ApiResource(name)
                select c;
            return Task.FromResult(api);
        }
    }
}
