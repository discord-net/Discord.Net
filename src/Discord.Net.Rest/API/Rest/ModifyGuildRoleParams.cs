#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildRoleParams
    {
        [ModelProperty("name")]
        public Optional<string> Name { get; set; }
        [ModelProperty("permissions")]
        public Optional<ulong> Permissions { get; set; }
        [ModelProperty("color")]
        public Optional<uint> Color { get; set; }
        [ModelProperty("hoist")]
        public Optional<bool> Hoist { get; set; }
        [ModelProperty("mentionable")]
        public Optional<bool> Mentionable { get; set; }
    }
}
