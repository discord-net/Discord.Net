#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class GetGuildsResponse
    {
        [ModelProperty("guilds")]
        public GuildSummary[] Guilds { get; set; }
    }
}
