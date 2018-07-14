using System.Net.Http;

namespace IdentityModelExtras
{
    public class NullDefaultHttpClientFactory : IDefaultHttpClientFactory
    {
        public HttpMessageHandler HttpMessageHandler { get; }
        public HttpClient HttpClient { get; }
    }
}