using Newtonsoft.Json;

namespace Discord.API
{
    internal class InviteEvent : InviteMetadata
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
