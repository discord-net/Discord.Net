using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("bitrate")]
        internal Optional<int> _bitrate;
        public int Bitrate { set { _bitrate = value; } }

        [JsonProperty("user_limit")]
        internal Optional<int> _userLimit;
        public int UserLimit { set { _userLimit = value; } }
    }
}
