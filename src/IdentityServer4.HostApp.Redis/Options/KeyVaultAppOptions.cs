﻿namespace IdentityServer4.HostApp.Options
{
    public class KeyVaultAppOptions
    {
        public bool UseKeyVault { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string KeyVaultUrl { get; set; }
        public string KeyIdentifier { get; set; }
        public string CronScheduleDataRefresh { get; set; }
    }
}