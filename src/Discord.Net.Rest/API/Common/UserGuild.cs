using Newtonsoft.Json;

namespace Discord.API
{
    internal class UserGuild
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("owner")]
        public bool Owner { get; set; }
        [JsonProperty("permissions"), Int53]
        public string Permissions { get; set; }
        [JsonProperty("features")]
        public GuildFeatures Features { get; set; }

        [JsonProperty("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; set; }

        [JsonProperty("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; set; }
    }
}
