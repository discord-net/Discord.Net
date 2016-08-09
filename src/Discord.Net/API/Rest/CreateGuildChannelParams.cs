#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateGuildChannelParams
    {
        [JsonProperty("name")]
        internal string _name { get; set; }
        public string Name { set { _name = value; } }

        [JsonProperty("type")]
        internal ChannelType _type { get; set; }
        public ChannelType Type { set { _type = value; } }

        [JsonProperty("bitrate")]
        internal Optional<int> _bitrate { get; set; }
        public int Bitrate { set { _bitrate = value; } }
    }
}
