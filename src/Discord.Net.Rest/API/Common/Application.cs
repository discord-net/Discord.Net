using Newtonsoft.Json;

namespace Discord.API
{
    internal class Application
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("rpc_origins")]
        public Optional<string[]> RPCOrigins { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("bot_public")]
        public bool IsBotPublic { get; set; }
        [JsonProperty("bot_require_code_grant")]
        public bool BotRequiresCodeGrant { get; set; }
        [JsonProperty("install_params")]
        public Optional<InstallParams> InstallParams { get; set; }
        [JsonProperty("team")]
        public Team Team { get; set; }
        [JsonProperty("flags"), Int53]
        public Optional<ApplicationFlags> Flags { get; set; }
        [JsonProperty("owner")]
        public Optional<User> Owner { get; set; }
        [JsonProperty("tags")]
        public Optional<string[]> Tags { get; set; }
        [JsonProperty("terms_of_service_url")]
        public string TermsOfService { get; set; }
        [JsonProperty("privacy_policy_url")]
        public string PrivacyPolicy { get; set; }

        [JsonProperty("custom_install_url")]
        public Optional<string> CustomInstallUrl { get; set; }

        [JsonProperty("role_connections_verification_url")]
        public Optional<string> RoleConnectionsUrl { get; set; }

        [JsonProperty("verify_key")]
        public string VerifyKey { get; set; }
    }
}
