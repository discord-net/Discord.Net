using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyVoiceChannelParams : ModifyTextChannelParams
    {
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }
        [JsonProperty("rtc_region")]
        public Optional<string> RTCRegion { get; set; }
    }
}
