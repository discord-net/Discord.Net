#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildRoleParams
    {
        [JsonProperty("name")]
        internal Optional<string> _name { get; set; }
        public string Name { set { _name = value; } }

        [JsonProperty("permissions")]
        internal Optional<ulong> _permissions { get; set; }
        public ulong Permissions { set { _permissions = value; } }

        [JsonProperty("position")]
        internal Optional<int> _position { get; set; }
        public int Position { set { _position = value; } }

        [JsonProperty("color")]
        internal Optional<uint> _color { get; set; }
        public uint Color { set { _color = value; } }

        [JsonProperty("hoist")]
        internal Optional<bool> _hoist { get; set; }
        public bool Hoist { set { _hoist = value; } }
    }
}
