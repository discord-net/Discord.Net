#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildMembersChunkEvent
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
        [ModelProperty("members")]
        public GuildMember[] Members { get; set; }
    }
}
