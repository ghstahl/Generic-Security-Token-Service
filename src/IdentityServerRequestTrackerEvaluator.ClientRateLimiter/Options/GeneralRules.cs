namespace IdentityServerRequestTrackerEvaluator.ClientRateLimiter.Options
{
    public class GeneralRules
    {
        public string Endpoint { get; set; }
        public string Period { get; set; }
        public int Limit { get; set; }
    }
}