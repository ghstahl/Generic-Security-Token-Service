using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.PlatformAbstractions;

namespace P7.Core.Reflection
{
    public class TypeGlobals
    {
        public static IEnumerable<ApplicationPart> ApplicationParts
        {
            get
            {
                return DefaultAssemblyPartDiscoveryProvider.DiscoverAssemblyParts(P7.Core.Global.HostingEnvironment.ApplicationName);
            }
        }
    }
}