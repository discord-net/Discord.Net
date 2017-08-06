#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildRoleCreateEvent
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
        [ModelProperty("role")]
        public Role Role { get; set; }
    }
}
