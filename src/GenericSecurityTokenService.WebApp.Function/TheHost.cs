using System.IO;
using GenericSecurityTokenService.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
 
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace GenericSecurityTokenService
{
    public static class TheHost
    {
        private static TestServer Server;

        public static TestServer GetServer(ExecutionContext context)
        {
            if (Server == null)
            {
                Server = new TestServer(new WebHostBuilder()
                    .UseContentRoot(context.FunctionAppDirectory)
                    .UseStartup<Startup>());
            }

            return Server;
        }

    }
}
