namespace P7IdentityServer4
{
    public class AzureKeyVaultTokenSigningServiceOptions
    {
        public string KeyIdentifier { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string KeyVaultUrl { get; set; }
    }
}