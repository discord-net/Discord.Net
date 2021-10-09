using Newtonsoft.Json;

namespace Discord.API
{
    internal class StageInstance
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("privacy_level")]
        public StagePrivacyLevel PrivacyLevel { get; set; }

        [JsonProperty("discoverable_disabled")]
        public bool DiscoverableDisabled { get; set; }
    }
}
