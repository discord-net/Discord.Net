using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildChannelParams
    {
        [JsonProperty("name")]
        internal Optional<string> _name;
        public string Name { set { _name = value; } }

        [JsonProperty("position")]
        internal Optional<int> _position;
        public int Position { set { _position = value; } }
    }
}
