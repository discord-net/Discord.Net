#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class GetGuildsResponse
    {
        [JsonProperty("guilds")]
        public GuildSummary[] Guilds { get; set; }
    }
}
