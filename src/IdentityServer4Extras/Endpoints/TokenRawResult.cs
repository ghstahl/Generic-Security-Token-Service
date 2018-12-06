using IdentityServer4.Endpoints.Results;

namespace IdentityServer4Extras.Endpoints
{
    public class TokenRawResult
    {
        public TokenErrorResult TokenErrorResult { get; set; }
        public TokenResult TokenResult { get; set; }
    }
    public class RevocationRawResult
    {
        public TokenRevocationErrorResult ErrorResult { get; set; }
        public StatusCodeResult StatusCodeResult { get; set; }
    }
}