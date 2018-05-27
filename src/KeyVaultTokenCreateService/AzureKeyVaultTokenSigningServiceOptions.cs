namespace P7IdentityServer4
{
    public class AzureKeyVaultTokenSigningServiceOptions
    {
        public string KeyIdentifier { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string KeyVaultUrl { get; set; }
//        https://crontab.guru/every-6-hours
        public string CronScheduleDataRefresh { get; set; } = "0 */6 * * *"; // “At minute 0 past every 6th hour.”
    }
}