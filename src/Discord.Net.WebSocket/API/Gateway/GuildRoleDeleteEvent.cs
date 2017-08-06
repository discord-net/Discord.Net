#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildRoleDeleteEvent
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
        [ModelProperty("role_id")]
        public ulong RoleId { get; set; }
    }
}
