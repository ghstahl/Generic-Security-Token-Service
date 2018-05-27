using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Serilog;

namespace P7.Core.Utils
{
    public class RemoteFetch
    {
        static Serilog.ILogger logger = Log.ForContext<RemoteFetch>();
        public static async Task<byte[]> FetchAsync(string url, WebRequestInit init)
        {
            try
            {
                var uri = url;
                var req = (HttpWebRequest)WebRequest.Create((string)uri);
                req.Accept = init.Accept;
                if (init.Headers != null)
                {

                    var allKeys = init.Headers.AllKeys;
                    foreach (var key in allKeys)
                    {
                        var value = init.Headers.Get(key);
                        req.Headers.Add(key, value);
                    }
                }
                var content = new MemoryStream();
                byte[] result;
                using (WebResponse response = await req.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {

                        // Read the bytes in responseStream and copy them to content.
                        await responseStream.CopyToAsync(content);
                        result =  content.ToArray();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                logger.Fatal("Exception Caught:{0}", e.Message);
            }
            return null;
        }
    }
}