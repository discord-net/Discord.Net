#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("bitrate")]
        internal Optional<int> _bitrate { get; set; }
        public int Bitrate { set { _bitrate = value; } }

        [JsonProperty("user_limit")]
        internal Optional<int> _userLimit { get; set; }
        public int UserLimit { set { _userLimit = value; } }
    }
}
