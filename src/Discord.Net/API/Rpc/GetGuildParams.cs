using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GetGuildParams
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
