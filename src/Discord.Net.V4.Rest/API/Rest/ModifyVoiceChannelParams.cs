using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyVoiceChannelParams : ModifyTextChannelParams
    {
        [JsonPropertyName("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonPropertyName("user_limit")]
        public Optional<int> UserLimit { get; set; }
        [JsonPropertyName("rtc_region")]
        public Optional<string> RTCRegion { get; set; }
    }
}
