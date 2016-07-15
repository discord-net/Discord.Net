using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GetGuildsResponse
    {
        [JsonProperty("guilds")]
        public RpcUserGuild[] Guilds { get; set; }
    }
}
