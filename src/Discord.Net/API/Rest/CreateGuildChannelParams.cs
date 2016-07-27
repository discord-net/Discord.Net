using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateGuildChannelParams
    {
        [JsonProperty("name")]
        internal string _name;
        public string Name { set { _name = value; } }

        [JsonProperty("type")]
        internal ChannelType _type;
        public ChannelType Type { set { _type = value; } }

        [JsonProperty("bitrate")]
        internal Optional<int> _bitrate;
        public int Bitrate { set { _bitrate = value; } }
    }
}
