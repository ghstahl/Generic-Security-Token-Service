using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Services;

namespace ProfileServiceManager
{
    public interface IProfileServicePlugin
    {
        string Name { get; }
        IProfileService ProfileService { get; }
    }
}
