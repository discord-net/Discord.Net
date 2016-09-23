#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class VoiceStateUpdateParams
    {
        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }
        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        public IGuild Guild { set { GuildId = value?.Id; } }

        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        public IChannel Channel { set { ChannelId = value?.Id; } }
    }
}
