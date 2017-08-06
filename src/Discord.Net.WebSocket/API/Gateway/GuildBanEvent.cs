#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildBanEvent
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
        [ModelProperty("user")]
        public User User { get; set; }
    }
}
