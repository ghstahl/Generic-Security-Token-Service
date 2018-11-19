namespace IdentityServerRequestTracker.RateLimit.Services
{
    public class ClientRequestIdentity
    {
        public string ClientId { get; set; }

        public string EndpointKey { get; set; }

    }
}