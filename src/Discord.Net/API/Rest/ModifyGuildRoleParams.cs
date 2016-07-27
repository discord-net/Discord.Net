using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildRoleParams
    {
        [JsonProperty("name")]
        internal Optional<string> _name;
        public string Name { set { _name = value; } }

        [JsonProperty("permissions")]
        internal Optional<ulong> _permissions;
        public ulong Permissions { set { _permissions = value; } }

        [JsonProperty("position")]
        internal Optional<int> _position;
        public int Position { set { _position = value; } }

        [JsonProperty("color")]
        internal Optional<uint> _color;
        public uint Color { set { _color = value; } }

        [JsonProperty("hoist")]
        internal Optional<bool> _hoist;
        public bool Hoist { set { _hoist = value; } }
    }
}
