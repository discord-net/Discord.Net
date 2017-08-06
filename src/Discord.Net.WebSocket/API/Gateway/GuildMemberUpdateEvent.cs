#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildMemberUpdateEvent : GuildMember
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
