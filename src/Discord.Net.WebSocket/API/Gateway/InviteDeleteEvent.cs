using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class InviteDeleteEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
    }
}
