using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace WizardAppApi.Services
{
    public class JsonFileLoader : IJsonFileLoader
    {
        private IHostingEnvironment _hostingEnvironment;

        public JsonFileLoader(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public object Load(string pathFragment)
        {
            var full = Path.Combine(_hostingEnvironment.WebRootPath, pathFragment);
            var stream = new FileStream(full, FileMode.Open);
            string fileContents;
            using (StreamReader reader = new StreamReader(stream))
            {
                fileContents = reader.ReadToEnd();
            }
            var result = JsonConvert.DeserializeObject(fileContents);
            return result;
        }
    }
}
