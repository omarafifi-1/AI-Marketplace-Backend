namespace AI_Marketplace.Application.Common.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = "fsdagfdsgdfsjhgksdfjhgjlsgrtkejghkhjsadfglasjfgasldjktvxczowerghlksadsfg";
        public string Issuer { get; set; } = "AIMarketplace";
        public string Audience { get; set; } = "AIMarketplaceUsers";
        public int ExpirationInDays { get; set; } = 30;
    }
}