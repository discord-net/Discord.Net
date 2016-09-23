#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class RpcGuild : Guild
    {
        [JsonProperty("online")]
        public int Online { get; set; }
        [JsonProperty("members")]
        public GuildMember[] Members { get; set; }
    }
}
