#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Role
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("color")]
        public uint Color { get; set; }
        [ModelProperty("hoist")]
        public bool Hoist { get; set; }
        [ModelProperty("mentionable")]
        public bool Mentionable { get; set; }
        [ModelProperty("position")]
        public int Position { get; set; }
        [ModelProperty("permissions"), Int53]
        public ulong Permissions { get; set; }
        [ModelProperty("managed")]
        public bool Managed { get; set; }
    }
}
