using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class UpdateVoiceParams
    {
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonProperty("self_mute")]
        public bool IsSelfMuted { get; set; }
        [JsonProperty("self_deaf")]
        public bool IsSelfDeafened { get; set; }
    }
}
