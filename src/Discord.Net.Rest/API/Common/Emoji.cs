#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Emoji
    {
        [ModelProperty("id")]
        public ulong? Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("roles")]
        public ulong[] Roles { get; set; }
        [ModelProperty("require_colons")]
        public bool RequireColons { get; set; }
        [ModelProperty("managed")]
        public bool Managed { get; set; }
    }
}
