using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class VoiceStateUpdateParams
    {
        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }
        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonIgnore]
        public IGuild Guild { set { GuildId = value.Id; } }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonIgnore]
        public IChannel Channel { set { ChannelId = value?.Id; } }
    }
}
