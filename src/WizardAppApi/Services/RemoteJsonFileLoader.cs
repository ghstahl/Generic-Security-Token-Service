using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using P7.Core.Utils;

namespace WizardAppApi.Services
{
    public class RemoteJsonFileLoader : IRemoteJsonFileLoader
    {
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;

        public RemoteJsonFileLoader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<object> LoadAsync(string pathFragment)
        {
            var domain = _configuration["remoteContent:domain"];
            var rootPathFragment = _configuration["remoteContent:rootPathFragment"];
            UriBuilder builder = new UriBuilder(domain) {Path = $"{rootPathFragment}/{pathFragment}"};
            var uri = builder.Uri;
            var fileContents = await RemoteJsonFetch.GetRemoteJsonContentAsync(uri.AbsoluteUri);
            var result = JsonConvert.DeserializeObject(fileContents);
            return result;
        }
    }
}