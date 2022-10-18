using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class VoiceStateUpdateParams
    {
        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; set; }
        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonPropertyName("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong? ChannelId { get; set; }
    }
}
