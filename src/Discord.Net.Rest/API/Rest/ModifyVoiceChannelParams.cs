using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        [JsonPropertyName("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonPropertyName("user_limit")]
        public Optional<int> UserLimit { get; set; }
        [JsonPropertyName("rtc_region")]
        public Optional<string> RTCRegion { get; set; }
    }
}
