using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Application
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("rpc_origins")]
        public Optional<string[]> RPCOrigins { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("bot_public")]
        public bool IsBotPublic { get; set; }
        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequiresCodeGrant { get; set; }
        [JsonPropertyName("install_params")]
        public Optional<InstallParams> InstallParams { get; set; }
        [JsonPropertyName("team")]
        public Team Team { get; set; }
        [JsonPropertyName("flags"), Int53]
        public Optional<ApplicationFlags> Flags { get; set; }
        [JsonPropertyName("owner")]
        public Optional<User> Owner { get; set; }
        [JsonPropertyName("tags")]
        public Optional<string[]> Tags { get; set; }
        [JsonPropertyName("terms_of_service_url")]
        public string TermsOfService { get; set; }
        [JsonPropertyName("privacy_policy_url")]
        public string PrivacyPolicy { get; set; }
    }
}
