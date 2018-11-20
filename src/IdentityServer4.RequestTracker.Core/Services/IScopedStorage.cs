using System.Collections.Generic;

namespace IdentityServerRequestTracker.Services
{
    public interface IScopedStorage
    {
        Dictionary<string,object> Storage { get; }
    }
}