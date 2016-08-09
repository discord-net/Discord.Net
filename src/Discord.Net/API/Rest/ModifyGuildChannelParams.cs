#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildChannelParams
    {
        [JsonProperty("name")]
        internal Optional<string> _name { get; set; }
        public string Name { set { _name = value; } }

        [JsonProperty("position")]
        internal Optional<int> _position { get; set; }
        public int Position { set { _position = value; } }
    }
}
