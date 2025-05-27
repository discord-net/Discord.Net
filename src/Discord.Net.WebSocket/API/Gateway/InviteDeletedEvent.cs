using Newtonsoft.Json;

namespace Discord.WebSocket
{
    internal class InviteDeletedEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildID { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
