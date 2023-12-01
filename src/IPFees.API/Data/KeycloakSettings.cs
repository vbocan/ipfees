namespace IPFees.API.Data
{
    public class KeycloakSettings
    {
        public const string SectionName = nameof(KeycloakSettings);
        public string Server { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string ClientID { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}