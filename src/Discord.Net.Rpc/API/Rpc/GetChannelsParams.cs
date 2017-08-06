#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class GetChannelsParams
    {
        [ModelProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
