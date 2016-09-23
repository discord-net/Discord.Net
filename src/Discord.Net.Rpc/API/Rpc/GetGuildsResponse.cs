#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GetGuildsResponse
    {
        [JsonProperty("guilds")]
        public RpcUserGuild[] Guilds { get; set; }
    }
}
