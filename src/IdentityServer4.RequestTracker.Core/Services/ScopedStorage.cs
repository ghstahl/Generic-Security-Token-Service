using System.Collections.Generic;

namespace IdentityServerRequestTracker.Services
{
    public class ScopedStorage : IScopedStorage
    {
        private Dictionary<string, object> _scopedStorage;

        public Dictionary<string, object> Storage => _scopedStorage ?? (_scopedStorage = new Dictionary<string, object>());
    }
}