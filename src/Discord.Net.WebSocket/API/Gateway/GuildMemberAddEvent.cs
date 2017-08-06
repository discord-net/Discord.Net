#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildMemberAddEvent : GuildMember
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
