namespace P7IdentityServer4
{
    public class AzureKeyVaultTokenSigningServiceOptions
    {
        public string KeyIdentifier { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string KeyVaultUrl { get; set; }
//        https://crontab.guru/every-6-hours
        public string CronScheduleDataRefresh { get; set; } = "*/5 * * * *"; //https://crontab.guru/every-5-minutes
    }
}