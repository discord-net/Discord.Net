using Newtonsoft.Json;

namespace Discord.API
{
    internal class Application
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("rpc_origins")]
        public string[] RPCOrigins { get; set; }
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
        [JsonProperty("team")]
        public Team Team { get; set; }

        [JsonProperty("flags"), Int53]
        public Optional<ulong> Flags { get; set; }
        [JsonProperty("owner")]
        public Optional<User> Owner { get; set; }
    }
}
