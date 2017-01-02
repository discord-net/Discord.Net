#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }
    }
}
